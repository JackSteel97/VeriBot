﻿using DSharpPlus;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VeriBot.Channels;
using VeriBot.DiscordModules.AuditLog.Services;
using VeriBot.Services;

namespace VeriBot.Channels.Voice;

public class VoiceStateChannel : BaseChannel<VoiceStateChange>
{
    private readonly AuditLogService _auditLogService;
    private readonly DiscordClient _discordClient;
    private readonly UserLockingService _userLockingService;
    private readonly UserTrackingService _userTrackingService;
    private readonly VoiceStateChangeHandler _voiceStateChangeHandler;

    public VoiceStateChannel(ILogger<VoiceStateChannel> logger,
        ErrorHandlingService errorHandlingService,
        VoiceStateChangeHandler voiceStateChangeHandler,
        UserTrackingService userTrackingService,
        DiscordClient discordClient,
        UserLockingService userLockingService,
        AuditLogService auditLogService) : base(logger, errorHandlingService, "Voice State")
    {
        _voiceStateChangeHandler = voiceStateChangeHandler;
        _userTrackingService = userTrackingService;
        _discordClient = discordClient;
        _userLockingService = userLockingService;
        _auditLogService = auditLogService;
    }

    protected override async ValueTask HandleMessage(VoiceStateChange message)
    {
        try
        {
            await _auditLogService.VoiceStateChange(message);
            using (await _userLockingService.WriterLockAsync(message.Guild.Id, message.User.Id))
            {
                if (await _userTrackingService.TrackUser(message.Guild.Id, message.User, message.Guild,
                        _discordClient))
                    await _voiceStateChangeHandler.HandleVoiceStateChange(message);
            }
        }
        catch (Exception e)
        {
            const string source = $"{nameof(VoiceStateChannel)}.{nameof(HandleMessage)}";
            await ErrorHandlingService.Log(e, source);
        }
    }
}