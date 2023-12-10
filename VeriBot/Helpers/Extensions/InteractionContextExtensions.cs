﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using VeriBot.Helpers;
using VeriBot.Services;

namespace VeriBot.Helpers.Extensions;

public static class InteractionContextExtensions
{
    public static void SendWarning(this InteractionContext context, string message, ErrorHandlingService errorHandlingService)
    {
        var response = new DiscordInteractionResponseBuilder().AsEphemeral().AddEmbed(EmbedGenerator.Warning(message));
        context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response).FireAndForget(errorHandlingService);
    }

    public static void SendMessage(this InteractionContext context, DiscordInteractionResponseBuilder response, ErrorHandlingService errorHandlingService) =>
        context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response).FireAndForget(errorHandlingService);

    public static void Acknowledge(this InteractionContext context, ErrorHandlingService errorHandlingService) =>
        context.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate).FireAndForget(errorHandlingService);

    public static void AcknowledgeThinking(this InteractionContext context, ErrorHandlingService errorHandlingService) =>
        context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource).FireAndForget(errorHandlingService);
}