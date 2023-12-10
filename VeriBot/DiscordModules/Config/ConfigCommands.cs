﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VeriBot.DiscordModules.AuditLog.Services;
using VeriBot.Helpers;
using VeriBot.Helpers.Extensions;

namespace VeriBot.DiscordModules.Config;

[Group("Configuration")]
[Description("Bot configuration commands.")]
[Aliases("config", "c")]
[RequireGuild]
public class ConfigCommands : TypingCommandModule
{
    private readonly DataHelpers _dataHelpers;
    private readonly ILogger<ConfigCommands> _logger;

    public ConfigCommands(DataHelpers dataHelpers, ILogger<ConfigCommands> logger, AuditLogService auditLogService) : base(logger, auditLogService)
    {
        _dataHelpers = dataHelpers;
        _logger = logger;
    }

    [Command("Environment")]
    [Aliases("env")]
    [Description("Gets the environment the bot is currently running in.")]
    [RequireOwner]
    [Cooldown(1, 300, CooldownBucketType.Channel)]
    public async Task Environment(CommandContext context) => await context.RespondAsync(EmbedGenerator.Primary($"I'm currently running in my **{_dataHelpers.Config.GetEnvironment()}** environment!"));

    [Command("Version")]
    [Aliases("v")]
    [Description("Displays the current version of the bot.")]
    [Cooldown(10, 120, CooldownBucketType.Channel)]
    public async Task Version(CommandContext context) => await context.RespondAsync(EmbedGenerator.Primary(_dataHelpers.Config.GetVersion(), "Version"));

    [Command("SetPrefix")]
    [Description("Sets the bot command prefix for this server.")]
    [RequireUserPermissions(Permissions.Administrator)]
    [Cooldown(1, 300, CooldownBucketType.Guild)]
    public async Task SetPrefix(CommandContext context, [RemainingText] string newPrefix)
    {
        if (newPrefix.Length > 20)
        {
            _logger.LogWarning("The new prefix must be 20 characters or less '{Prefix}' was too long", newPrefix);
            await context.RespondAsync(EmbedGenerator.Error("The new prefix must be 20 characters or less."));
            return;
        }

        if (string.IsNullOrWhiteSpace(newPrefix))
        {
            _logger.LogWarning("No prefix was specified");
            await context.RespondAsync(EmbedGenerator.Error("No valid prefix specified."));
            return;
        }

        await _dataHelpers.Config.SetPrefix(context.Guild.Id, newPrefix);
        await context.RespondAsync(EmbedGenerator.Success($"Prefix changed to **{newPrefix}**"));
    }

    [Command("ToggleDadJoke")]
    [Description("Toggles Dad Joke Detector on/off for this server.")]
    [RequireUserPermissions(Permissions.Administrator)]
    [Cooldown(2, 300, CooldownBucketType.Guild)]
    public async Task ToggleDadJoke(CommandContext context)
    {
        bool newSetting = await _dataHelpers.Config.ToggleDadJoke(context.Guild.Id);
        await context.RespondAsync(EmbedGenerator.Success($"Dad Joke Detector Toggled **{(newSetting ? "On" : "Off")}**"));
    }

    [Command("SetLevelChannel")]
    [Aliases("SetAnnouncementChannel", "SetChannel", "SetLC", "SetAC")]
    [Description("Set the channel to use to notify users of level-ups")]
    [RequireUserPermissions(Permissions.Administrator)]
    [Cooldown(1, 300, CooldownBucketType.Guild)]
    public async Task SetLevelChannel(CommandContext context, DiscordChannel channel)
    {
        if (channel == null || !context.Guild.Channels.ContainsKey(channel.Id))
        {
            _logger.LogWarning("Invalid channel entered for setting the levelling channel");
            await context.RespondAsync(EmbedGenerator.Error("That channel is not valid."));
            return;
        }

        await _dataHelpers.Config.SetLevellingChannel(context.Guild.Id, channel.Id);
        await context.RespondAsync(EmbedGenerator.Success($"Levelling channel set to {channel.Mention}"));
    }
}