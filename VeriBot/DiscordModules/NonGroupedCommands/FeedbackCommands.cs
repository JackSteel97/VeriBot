using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VeriBot.DataProviders;
using VeriBot.DiscordModules.AuditLog.Services;
using VeriBot.Helpers;
using VeriBot.Helpers.Extensions;

namespace VeriBot.DiscordModules.NonGroupedCommands;

[Description("Commands for providing feedback about the bot.")]
[RequireGuild]
public class FeedbackCommands : TypingCommandModule
{
    private readonly DataCache _cache;

    public FeedbackCommands(DataCache cache, ILogger<FeedbackCommands> logger, AuditLogService auditLogService) : base(logger, auditLogService)
    {
        _cache = cache;
    }

    [Command("Good")]
    [Description("Adds a good bot vote.")]
    [Cooldown(1, 60, CooldownBucketType.User)]
    public async Task GoodBot(CommandContext context, [RemainingText] string remainder)
    {
        if (remainder != null && remainder.Equals("bot", StringComparison.OrdinalIgnoreCase))
        {
            await _cache.Guilds.IncrementGoodVote(context.Guild.Id);
            await context.RespondAsync(EmbedGenerator.Info("Thank you!"));
        }
    }

    [Command("Bad")]
    [Description("Adds a bad bot vote.")]
    [Cooldown(1, 60, CooldownBucketType.User)]
    public async Task BadBot(CommandContext context, [RemainingText] string remainder)
    {
        if (remainder != null && remainder.Equals("bot", StringComparison.OrdinalIgnoreCase))
        {
            await _cache.Guilds.IncrementBadVote(context.Guild.Id);
            await context.RespondAsync(EmbedGenerator.Info("I'm sorry!"));
        }
    }
}