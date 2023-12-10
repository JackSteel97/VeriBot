﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeriBot.Database;
using VeriBot.Database.Models;
using VeriBot.Database.Models.Users;

namespace VeriBot.DataProviders.SubProviders;

public class TriggersProvider
{
    private readonly IDbContextFactory<VeriBotContext> _dbContextFactory;
    private readonly ILogger<TriggersProvider> _logger;
    private readonly Dictionary<ulong, Dictionary<string, Trigger>> _triggersByGuild;

    public TriggersProvider(ILogger<TriggersProvider> logger, IDbContextFactory<VeriBotContext> contextFactory)
    {
        _logger = logger;
        _dbContextFactory = contextFactory;

        _triggersByGuild = new Dictionary<ulong, Dictionary<string, Trigger>>();
        LoadTriggersData();
    }

    public void LoadTriggersData()
    {
        _logger.LogInformation("Loading data from database: Triggers");
        Trigger[] allTriggers;
        using (var db = _dbContextFactory.CreateDbContext())
        {
            allTriggers = db.Triggers.AsNoTracking().Include(t => t.Guild).Include(t => t.Creator).ToArray();
        }

        foreach (var trigger in allTriggers) AddTriggerToInternalCache(trigger.Guild.DiscordId, trigger);
    }

    private void AddTriggerToInternalCache(ulong guildId, Trigger trigger, User creator = null)
    {
        if (!_triggersByGuild.TryGetValue(guildId, out var triggers))
        {
            triggers = new Dictionary<string, Trigger>();
            _triggersByGuild.Add(guildId, triggers);
        }

        if (!triggers.ContainsKey(trigger.TriggerText.ToLower()))
        {
            if (creator != null) trigger.Creator = creator;
            triggers.Add(trigger.TriggerText.ToLower(), trigger);
        }
    }

    private void RemoveTriggerFromInternalCache(ulong guildId, string trigger)
    {
        if (_triggersByGuild.TryGetValue(guildId, out var triggers))
        {
            string key = trigger.ToLower();
            if (triggers.ContainsKey(key)) triggers.Remove(key);
        }
    }

    public bool BotKnowsTrigger(ulong guildId, string trigger) => _triggersByGuild.TryGetValue(guildId, out var roles) && roles.ContainsKey(trigger.ToLower());

    public bool TryGetTrigger(ulong guildId, string triggerText, out Trigger trigger)
    {
        if (_triggersByGuild.TryGetValue(guildId, out var triggers)) return triggers.TryGetValue(triggerText.ToLower(), out trigger);
        trigger = null;
        return false;
    }

    public bool TryGetGuildTriggers(ulong guildId, out Dictionary<string, Trigger> triggers) => _triggersByGuild.TryGetValue(guildId, out triggers);

    public async Task AddTrigger(ulong guildId, Trigger trigger, User creator)
    {
        if (!BotKnowsTrigger(guildId, trigger.TriggerText)) await InsertTrigger(guildId, trigger, creator);
    }

    public async Task RemoveTrigger(ulong guildId, string triggerText)
    {
        if (TryGetTrigger(guildId, triggerText, out var trigger)) await DeleteTrigger(guildId, trigger);
    }

    public async Task IncrementActivations(ulong guildId, Trigger trigger)
    {
        trigger.TimesActivated++;
        await UpdateTrigger(guildId, trigger);
    }

    private async Task InsertTrigger(ulong guildId, Trigger trigger, User creator)
    {
        _logger.LogInformation($"Writing a new Trigger [{trigger.TriggerText}] for Guild [{guildId}] to the database");
        int writtenCount;
        using (var db = _dbContextFactory.CreateDbContext())
        {
            db.Triggers.Add(trigger);
            writtenCount = await db.SaveChangesAsync();
        }

        if (writtenCount > 0)
            AddTriggerToInternalCache(guildId, trigger, creator);
        else
            _logger.LogError($"Writing Trigger [{trigger.TriggerText}] for Guild [{guildId}] to the database inserted no entities. The internal cache was not changed");
    }

    private async Task UpdateTrigger(ulong guildId, Trigger newTrigger)
    {
        _logger.LogInformation($"Updating Trigger [{newTrigger.TriggerText}] for Guild [{guildId}] in the database");

        int writtenCount;
        using (var db = _dbContextFactory.CreateDbContext())
        {
            // To prevent EF tracking issue, grab and alter existing value.
            var original = db.Triggers.First(u => u.RowId == newTrigger.RowId);
            db.Entry(original).CurrentValues.SetValues(newTrigger);
            db.Triggers.Update(original);
            writtenCount = await db.SaveChangesAsync();
        }

        if (writtenCount > 0)
            _triggersByGuild[guildId][newTrigger.TriggerText.ToLower()] = newTrigger;
        else
            _logger.LogError($"Updating Trigger [{newTrigger.TriggerText}] in Guild [{guildId}] did not alter any entities. The internal cache was not changed.");
    }

    private async Task DeleteTrigger(ulong guildId, Trigger trigger)
    {
        _logger.LogInformation($"Deleting Trigger [{trigger.TriggerText}] for Guild [{guildId}] from the database.");

        int writtenCount;
        using (var db = _dbContextFactory.CreateDbContext())
        {
            // Remove creator to prevent EF trying to deleting things.
            trigger.Creator = null;
            db.Triggers.Remove(trigger);
            writtenCount = await db.SaveChangesAsync();
        }

        if (writtenCount > 0)
            RemoveTriggerFromInternalCache(guildId, trigger.TriggerText);
        else
            _logger.LogWarning($"Deleting Trigger [{trigger.TriggerText}] for Guild [{guildId}] from the database deleted no entities. The internal cache was not changed.");
    }
}