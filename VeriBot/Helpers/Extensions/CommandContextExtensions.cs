﻿using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace VeriBot.Helpers.Extensions;

public static class CommandContextExtensions
{
    public static Task<DiscordMessage> RespondAsync(this CommandContext context, DiscordEmbed embed, bool mention)
    {
        var message = new DiscordMessageBuilder().WithEmbed(embed);
        return context.RespondAsync(message, mention);
    }

    public static Task<DiscordMessage> RespondAsync(this CommandContext context, DiscordEmbedBuilder embed, bool mention)
    {
        var message = new DiscordMessageBuilder().WithEmbed(embed);
        return context.RespondAsync(message, mention);
    }

    public static Task<DiscordMessage> RespondAsync(this CommandContext context, DiscordMessageBuilder messageBuilder, bool mention) =>
        context.Channel.SendMessageAsync(messageBuilder.WithReply(context.Message.Id, mention));
}