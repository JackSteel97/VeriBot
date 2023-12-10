﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Humanizer;
using System.Collections.Generic;
using System.Text;
using VeriBot.Database.Models.AuditLog;
using VeriBot.Helpers;
using VeriBot.Helpers.Extensions;

namespace VeriBot.DiscordModules.AuditLog.Services;

public class AuditLogViewingService
{
    public static List<Page> BuildViewResponsePages(DiscordGuild guild, IEnumerable<Audit> audits)
    {
        var baseEmbed = new DiscordEmbedBuilder().WithColor(EmbedGenerator.InfoColour).WithTitle($"{guild.Name} Audit Log");
        var pages = PaginationHelper.GenerateEmbedPages(baseEmbed, audits, 5, (builder, audit, _) => AppendEntry(builder, audit));
        return pages;
    }

    private static StringBuilder AppendEntry(StringBuilder builder, Audit entry)
    {
        builder.AppendLine(Formatter.Bold(entry.What.Humanize()))
            .AppendLine($"When?: `{entry.When.ToString("g")}`")
            .AppendLine($"Who?: {entry.Who.ToUserMention()}  (`{entry.WhoName}`)");

        if (entry.WhereChannelId.HasValue) builder.AppendLine($"Where?: `{entry.WhereChannelName}`");

        return builder;
    }
}