using DSharpPlus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using VeriBot.Channels.Message;
using VeriBot.Channels.Pets;
using VeriBot.Channels.Puzzle;
using VeriBot.Channels.RankRole;
using VeriBot.Channels.SelfRole;
using VeriBot.Channels.Stats;
using VeriBot.Channels.Voice;
using VeriBot.Database;
using VeriBot.DataProviders;
using VeriBot.DataProviders.SubProviders;
using VeriBot.DiscordModules;
using VeriBot.DiscordModules.AuditLog.Services;
using VeriBot.DiscordModules.Config;
using VeriBot.DiscordModules.Fun;
using VeriBot.DiscordModules.Pets;
using VeriBot.DiscordModules.Pets.Generation;
using VeriBot.DiscordModules.Pets.Services;
using VeriBot.DiscordModules.Puzzle.Questions;
using VeriBot.DiscordModules.Puzzle.Services;
using VeriBot.DiscordModules.RankRoles.Services;
using VeriBot.DiscordModules.SelfRoles;
using VeriBot.DiscordModules.SelfRoles.Services;
using VeriBot.DiscordModules.Stats;
using VeriBot.DiscordModules.Stats.Services;
using VeriBot.DiscordModules.Triggers;
using VeriBot.DiscordModules.Utility;
using VeriBot.DSharpPlusOverrides;
using VeriBot.Helpers;
using VeriBot.Helpers.Levelling;
using VeriBot.RateLimiting;
using VeriBot.Services;
using VeriBot.Services.Configuration;

namespace VeriBot;

public static class Program
{
    private static readonly string _environment = Environment.GetEnvironmentVariable("VERIBOTENVIRONMENT") ?? "Production";

    private static IServiceProvider ConfigureServices(IServiceCollection serviceProvider)
    {
        serviceProvider.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(30));
        // Config setup.
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{_environment.ToLower()}.json", false, true)
            .Build();

        var appConfigurationService = new AppConfigurationService();
        configuration.Bind("AppConfig", appConfigurationService);
        appConfigurationService.Environment = _environment;
        appConfigurationService.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        appConfigurationService.BasePath = Directory.GetCurrentDirectory();
        appConfigurationService.StartUpTime = DateTime.UtcNow;

        serviceProvider.AddSingleton(appConfigurationService);

        // Set static dependency
        UserExtensions.LevelConfig = appConfigurationService.Application.Levelling;

        // Logging setup.
        const int fileSizeLimitBytes = 8 * 1000 * 1000;
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.File("Logs/VeriBotLog.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: fileSizeLimitBytes, rollOnFileSizeLimit: true);
#if DEBUG
        loggerConfig.WriteTo.Console();
#endif
        Log.Logger = loggerConfig.CreateLogger();
        Log.Logger.Information("Logger Created");

        try
        {
            var loggerFactory = new LoggerFactory().AddSerilog();
            serviceProvider.AddLogging(opt =>
            {
                opt.ClearProviders();
                opt.AddSerilog(Log.Logger);
                opt.AddConfiguration(configuration);
            });

            // Database DI.
            serviceProvider.AddPooledDbContextFactory<VeriBotContext>(options => options.UseNpgsql(appConfigurationService.Database.ConnectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                        .EnableRetryOnFailure(10))
                .EnableSensitiveDataLogging(_environment.Equals("Development"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution));

            ConfigureCustomServices(serviceProvider);
            ConfigureDataProviders(serviceProvider);
            ConfigureDataHelpers(serviceProvider);

            // Discord client setup.
            var client = new DiscordClient(new DiscordConfiguration
            {
                LoggerFactory = loggerFactory,
                MinimumLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), appConfigurationService.Application.Discord.LogLevel),
                MessageCacheSize = appConfigurationService.Application.Discord.MessageCacheSize,
                Token = appConfigurationService.Application.Discord.LoginToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers | DiscordIntents.MessageContents
            });

            serviceProvider.AddSingleton(client);

            // Main app.
            serviceProvider.AddHostedService<BotMain>();

            return serviceProvider.BuildServiceProvider(true);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "A Fatal exception occurred during startup");
            throw;
        }
    }

    private static void ConfigureDataHelpers(IServiceCollection serviceProvider)
    {
        // Add data helpers.
        serviceProvider.AddSingleton<UserTrackingService>();
        serviceProvider.AddSingleton<StatsDataHelper>();
        serviceProvider.AddSingleton<ConfigDataHelper>();
        serviceProvider.AddSingleton<RolesDataHelper>();
        serviceProvider.AddSingleton<TriggerDataHelper>();
        serviceProvider.AddSingleton<FunDataHelper>();
        serviceProvider.AddSingleton<PetsDataHelper>();
        // Add base provider.
        serviceProvider.AddSingleton<DataHelpers>();
    }

    private static void ConfigureDataProviders(IServiceCollection serviceProvider)
    {
        // Add data providers.
        serviceProvider.AddSingleton<GuildsProvider>();
        serviceProvider.AddSingleton<UsersProvider>();
        serviceProvider.AddSingleton<SelfRolesProvider>();
        serviceProvider.AddSingleton<ExceptionProvider>();
        serviceProvider.AddSingleton<RankRolesProvider>();
        serviceProvider.AddSingleton<TriggersProvider>();
        serviceProvider.AddSingleton<CommandStatisticProvider>();
        serviceProvider.AddSingleton<FunProvider>();
        serviceProvider.AddSingleton<PetsProvider>();
        serviceProvider.AddSingleton<PuzzleProvider>();

        // Add base provider.
        serviceProvider.AddSingleton<DataCache>();
    }

    private static void ConfigureCustomServices(IServiceCollection serviceProvider)
    {
        // TODO: Scope services?
        // Add custom services.
        serviceProvider.AddSingleton<UserTrackingService>();
        serviceProvider.AddSingleton<LevelCardGenerator>();
        serviceProvider.AddSingleton<PetFactory>();

        serviceProvider.AddSingleton<ErrorHandlingService>();
        serviceProvider.AddSingleton<CancellationService>();

        serviceProvider.AddSingleton<VoiceStateChannel>();
        serviceProvider.AddSingleton<VoiceStateChangeHandler>();
        serviceProvider.AddSingleton<LevelMessageSender>();

        serviceProvider.AddSingleton<MessagesChannel>();
        serviceProvider.AddSingleton<IncomingMessageHandler>();

        serviceProvider.AddSingleton<SelfRoleManagementChannel>();
        serviceProvider.AddSingleton<SelfRoleCreationService>();
        serviceProvider.AddSingleton<SelfRoleMembershipService>();
        serviceProvider.AddSingleton<SelfRoleViewingService>();

        serviceProvider.AddSingleton<RankRoleManagementChannel>();
        serviceProvider.AddSingleton<RankRoleCreationService>();
        serviceProvider.AddSingleton<RankRoleDeletionService>();
        serviceProvider.AddSingleton<RankRoleViewingService>();

        serviceProvider.AddSingleton<PetCommandsChannel>();
        serviceProvider.AddSingleton<PetViewingService>();
        serviceProvider.AddSingleton<PetBonusViewingService>();
        serviceProvider.AddSingleton<PetBefriendingService>();
        serviceProvider.AddSingleton<PetSearchingService>();
        serviceProvider.AddSingleton<PetManagementService>();
        serviceProvider.AddSingleton<PetTreatingService>();
        serviceProvider.AddSingleton<PetDeathService>();

        serviceProvider.AddSingleton<UserLockingService>();
        serviceProvider.AddSingleton<ErrorHandlingAsynchronousCommandExecutor>();

        serviceProvider.AddSingleton<StatsCommandsChannel>();
        serviceProvider.AddSingleton<StatsAdminService>();
        serviceProvider.AddSingleton<StatsCardService>();
        serviceProvider.AddSingleton<StatsLeaderboardService>();

        serviceProvider.AddSingleton<PuzzleCommandsChannel>();
        serviceProvider.AddSingleton<PuzzleService>();
        serviceProvider.AddSingleton<QuestionFactory>();

        serviceProvider.AddMemoryCache();
        serviceProvider.AddSingleton<RateLimitFactory>();

        serviceProvider.AddSingleton<UtilityService>();

        serviceProvider.AddSingleton<AuditLogProvider>();
        serviceProvider.AddSingleton<AuditLogService>();
    }

    public static async Task Main(string[] args)
    {
        try
        {
            await CreateHostBuilder(args).UseConsoleLifetime().Build().RunAsync();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var serviceProvider = ConfigureServices(services);
            });
}