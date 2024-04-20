﻿using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VeriBot.Database.Models.Users;

namespace VeriBot.Database.Models;

public class Guild
{
    public long RowId { get; set; }

    public ulong DiscordId { get; set; }

    public DateTime BotAddedTo { get; set; }

    public List<User> UsersInGuild { get; set; }
    public List<SelfRole> SelfRoles { get; set; }
    public List<RankRole> RankRoles { get; set; }

    public List<Trigger> Triggers { get; set; }

    [MaxLength(20)] public string CommandPrefix { get; set; }

    public ulong? LevelAnnouncementChannelId { get; set; }

    public int GoodBotVotes { get; set; }
    public int BadBotVotes { get; set; }

    [MaxLength(255)] public string Name { get; set; }

    public bool DadJokesEnabled { get; set; }
    
    public ulong? SelfRolesAssignmentMessageId { get; set; }
    public ulong? SelfRolesAssignmentMessageChannelId { get; set; }

    /// <summary>
    ///     Empty constructor.
    ///     Do not remove - used by EF.
    /// </summary>
    public Guild() { }

    public Guild(ulong discordId, string name)
    {
        DiscordId = discordId;
        BotAddedTo = DateTime.UtcNow;
        Name = name;
    }

    public Guild Clone()
    {
        var guildCopy = (Guild)MemberwiseClone();
        return guildCopy;
    }

    public DiscordChannel GetLevelAnnouncementChannel(DiscordGuild discordGuild) =>
        LevelAnnouncementChannelId.HasValue ? discordGuild.GetChannel(LevelAnnouncementChannelId.Value) : discordGuild?.SystemChannel;
}