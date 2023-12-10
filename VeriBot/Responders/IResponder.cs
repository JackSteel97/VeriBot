﻿using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeriBot.Helpers.Extensions;
using VeriBot.Helpers.Interactivity.Models;

namespace VeriBot.Responders;

public interface IResponder
{
    /// <summary>
    ///     Respond to the user with the provided message.
    /// </summary>
    /// <param name="messageBuilder">The message to send.</param>
    /// <param name="ephemeral">Only shown to the executor of the command - has no effect on message responders.</param>
    /// <returns>A task that completes when the message is sent containing the sent message./</returns>
    Task<DiscordMessage> RespondAsync(DiscordMessageBuilder messageBuilder, bool ephemeral = false);

    /// <summary>
    ///     Respond to the user with the provided message.
    /// </summary>
    /// <remarks>
    ///     Implementers should not block the thread waiting for the message to send.
    ///     Instead use <see cref="TaskExtensions.FireAndForget" /> to ensure any errors are
    ///     handled and avoid blocking the thread.
    /// </remarks>
    /// <param name="messageBuilder">The message to send.</param>
    /// <param name="ephemeral">Only shown to the executor of the command - has no effect on message responders.</param>
    void Respond(DiscordMessageBuilder messageBuilder, bool ephemeral = false);

    /// <summary>
    ///     Respond to the user with a paginated message.
    /// </summary>
    /// <param name="pages">The pages to use.</param>
    /// <returns>A task that completes when the pagination interactivity ends.</returns>
    Task RespondPaginatedAsync(List<Page> pages);

    /// <summary>
    ///     Respond to the user with the provided message.
    /// </summary>
    /// <remarks>
    ///     Implementers should not block the thread waiting for the message to send.
    ///     Instead use <see cref="TaskExtensions.FireAndForget" /> to ensure any errors are
    ///     handled and avoid blocking the thread.
    /// </remarks>
    /// <param name="pages">The pages to use.</param>
    void RespondPaginated(List<Page> pages);

    Task<(string selectionId, DiscordInteraction interaction)> RespondPaginatedWithComponents(List<PageWithSelectionButtons> pages);

    /// <summary>
    ///     Set the interaction that should be used from now on if any is required.
    /// </summary>
    /// <param name="interaction">Interaction to use</param>
    void SetInteraction(DiscordInteraction interaction);
}