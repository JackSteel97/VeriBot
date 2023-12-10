﻿using DSharpPlus.Entities;
using VeriBot.Channels;
using VeriBot.Responders;

namespace VeriBot.Channels.Stats;

public enum StatsCommandActionType
{
    ViewPersonalStats,
    ViewBreakdown,
    ViewMetricLeaderboard,
    ViewLevelsLeaderboard,
    ViewAll,
    ViewVelocity
}

public record StatsCommandAction : BaseAction<StatsCommandActionType>
{
    public DiscordMember Target { get; }

    public string Metric { get; }

    public long Top { get; }

    public StatsCommandAction(StatsCommandActionType action, IResponder responder, DiscordMember member, DiscordGuild guild, DiscordMember target = null, string metric = null, long top = 0)
        : base(action, responder, member, guild)
    {
        Target = target ?? member;
        Metric = metric;
        Top = top;
    }
}