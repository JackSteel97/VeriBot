﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using VeriBot.Database;
using VeriBot.Database.Models.AuditLog;

namespace VeriBot.DataProviders.SubProviders;

public class AuditLogProvider
{
    private readonly ILogger<AuditLogProvider> _logger;
    private readonly IDbContextFactory<VeriBotContext> _dbContextFactory;

    public AuditLogProvider(ILogger<AuditLogProvider> logger, IDbContextFactory<VeriBotContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task Write(Audit auditLogItem)
    {
        _logger.LogInformation("Writing a new {Action} item to the audit log for {UserId}", auditLogItem.What, auditLogItem.Who);

        int writtenCount;
        await using (var db = await _dbContextFactory.CreateDbContextAsync())
        {
            db.AuditLog.Add(auditLogItem);
            writtenCount = await db.SaveChangesAsync();
        }

        if (writtenCount == 0)
            _logger.LogError("Writing a new {Action} item to the audit log for {UserId} instered no entries", auditLogItem.What, auditLogItem.Who);
    }

    public async Task<Audit[]> GetLatest(ulong guildId, AuditAction? action = null)
    {
        _logger.LogInformation("Reading the latest entries in the audit log for Guild {GuildId}", guildId);

        Audit[] results;
        await using (var db = await _dbContextFactory.CreateDbContextAsync())
        {
            results = await db.AuditLog
                .Where(a => a.WhereGuildId == guildId && (action == null || a.What == action))
                .OrderByDescending(a => a.When)
                .Take(50)
                .ToArrayAsync();
        }

        return results;
    }
}