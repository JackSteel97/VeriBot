﻿using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeriBot.Channels.Voice;
using VeriBot.Services;

namespace VeriBot.Channels.Message;

public class MessagesChannel : BaseChannel<IncomingMessage>
{
    private static readonly TimeSpan _voiceUpdateTimeout = TimeSpan.FromMinutes(1);
    private readonly DiscordClient _discordClient;
    private readonly IncomingMessageHandler _incomingMessageHandler;
    private readonly Dictionary<(ulong guildId, ulong userId), DateTime> _lastVoiceUpdateFromMessage;
    private readonly UserLockingService _userLockingService;
    private readonly UserTrackingService _userTrackingService;
    private readonly VoiceStateChangeHandler _voiceStateChangeHandler;

    public MessagesChannel(ILogger<MessagesChannel> logger,
        ErrorHandlingService errorHandlingService,
        UserTrackingService userTrackingService,
        DiscordClient discordClient,
        IncomingMessageHandler incomingMessageHandler,
        VoiceStateChangeHandler voiceStateChangeHandler,
        UserLockingService userLockingService) : base(logger, errorHandlingService, "Messages")
    {
        _userTrackingService = userTrackingService;
        _discordClient = discordClient;
        _incomingMessageHandler = incomingMessageHandler;
        _voiceStateChangeHandler = voiceStateChangeHandler;
        _userLockingService = userLockingService;
        _lastVoiceUpdateFromMessage = new Dictionary<(ulong guildId, ulong userId), DateTime>();
    }

    protected override async ValueTask HandleMessage(IncomingMessage message)
    {
        try
        {
            using (await _userLockingService.WriterLockAsync(message.Guild.Id, message.User.Id))
            {
                if (await _userTrackingService.TrackUser(message.Guild.Id, message.User, message.Guild,
                        _discordClient))
                {
                    await _incomingMessageHandler.HandleMessage(message);

                    var key = (message.Guild.Id, message.User.Id);
                    if (!_lastVoiceUpdateFromMessage.TryGetValue(key, out var lastUpdate) ||
                        DateTime.UtcNow - lastUpdate > _voiceUpdateTimeout)
                    {
                        _lastVoiceUpdateFromMessage[key] = DateTime.UtcNow;

                        // The user here is already coming from a Guild so we can safely cast to a member.
                        var member = (DiscordMember)message.User;
                        await _voiceStateChangeHandler.HandleVoiceStateChange(
                            new VoiceStateChange(message.Guild, message.User, member.VoiceState));
                    }
                }
            }
        }
        catch (Exception e)
        {
            const string source = $"{nameof(MessagesChannel)}.{nameof(HandleMessage)}";
            await ErrorHandlingService.Log(e, source);
        }
    }
}