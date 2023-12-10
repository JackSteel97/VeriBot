﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VeriBot.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
public sealed class GuildCheckAttribute : CheckBaseAttribute
{
    private HashSet<ulong> AllowedServerIds { get; }

    public GuildCheckAttribute(params ulong[] allowedServerIds)
    {
        AllowedServerIds = allowedServerIds.ToHashSet();
    }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        bool allowed = ctx.Guild != null && AllowedServerIds.Contains(ctx.Guild.Id);
        return Task.FromResult(allowed);
    }
}