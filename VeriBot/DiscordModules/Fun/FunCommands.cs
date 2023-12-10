using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VeriBot.Database.Models;
using VeriBot.DataProviders;
using VeriBot.DiscordModules.AuditLog.Services;
using VeriBot.Helpers;
using VeriBot.Helpers.Extensions;

namespace VeriBot.DiscordModules.Fun;

[Group("Fun")]
[Aliases("f")]
[Description("Commands for fun.")]
[RequireGuild]
public class FunCommands : TypingCommandModule
{
    private readonly DataCache _cache;
    private readonly ILogger<FunCommands> _logger;

    public FunCommands( DataCache cache, ILogger<FunCommands> logger, AuditLogService auditLogService) : base(logger, auditLogService)
    {
        _cache = cache;
        _logger = logger;
    }

    [Command("Inspo")]
    [Aliases("Inspiration", "Motivate", "Quote")]
    [Description("Gets an AI generated motivational quote")]
    [Cooldown(5, 60, CooldownBucketType.User)]
    public async Task GetInspiration(CommandContext context)
    {
        var imageStream = await FunDataHelper.GetMotivationalQuote();

        if (imageStream != null)
        {
            var msg = new DiscordMessageBuilder().AddFile("MotivationalQuote.jpg", imageStream);
            await context.RespondAsync(msg);
        }
        else
        {
            _logger.LogWarning("Failed to generate a motivational quote");
            await context.RespondAsync(EmbedGenerator.Error("Failed to generate a motivational quote, please try again later."));
            await _cache.Exceptions.InsertException(new ExceptionLog(new NullReferenceException("Motivational Quote stream cannot be null"), nameof(GetInspiration)));
        }
    }
}