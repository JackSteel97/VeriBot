using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VeriBot.Database.Models;
using VeriBot.DataProviders.SubProviders;
using VeriBot.DiscordModules.AuditLog.Services;
using VeriBot.Helpers;
using VeriBot.Helpers.Extensions;
using VeriBot.Responders;
using VeriBot.Services;

namespace VeriBot.DiscordModules.Fun;

[SlashCommandGroup("Fun", "Commands for fun")]
[SlashRequireGuild]
public class FunSlashCommands : InstrumentedApplicationCommandModule
{
    private readonly ErrorHandlingService _errorHandlingService;
    private readonly ExceptionProvider _exceptionProvider;
    private readonly ILogger<FunSlashCommands> _logger;

    /// <inheritdoc />
    public FunSlashCommands(
        ILogger<FunSlashCommands> logger,
        ErrorHandlingService errorHandlingService,
        ExceptionProvider exceptionProvider,
        AuditLogService auditLogService) : base(logger, auditLogService)
    {
        _logger = logger;
        _errorHandlingService = errorHandlingService;
        _exceptionProvider = exceptionProvider;
    }

    [SlashCommand("Inspiration", "Gets an AI generated motivational quote image")]
    [SlashCooldown(5, 60, SlashCooldownBucketType.User)]
    public async Task GetInspiration(InteractionContext context)
    {
        var responder = new InteractionResponder(context, _errorHandlingService);
        var imageStream = await FunDataHelper.GetMotivationalQuote();

        if (imageStream != null)
        {
            var msg = new DiscordMessageBuilder().AddFile("MotivationalQuote.jpg", imageStream);
            responder.Respond(msg);
        }
        else
        {
            _logger.LogWarning("Failed to generate a motivational quote");
            responder.Respond(new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("Failed to generate a motivational quote, please try again later.")), true);
            await _exceptionProvider.InsertException(new ExceptionLog(new NullReferenceException("Motivational Quote stream cannot be null"), nameof(GetInspiration)));
        }
    }
}