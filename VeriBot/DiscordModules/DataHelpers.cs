using VeriBot.DiscordModules.Config;
using VeriBot.DiscordModules.Fun;
using VeriBot.DiscordModules.Pets;
using VeriBot.DiscordModules.SelfRoles;
using VeriBot.DiscordModules.Stats;
using VeriBot.DiscordModules.Triggers;

namespace VeriBot.DiscordModules;

public class DataHelpers
{
    public StatsDataHelper Stats { get; }

    public ConfigDataHelper Config { get; }

    public RolesDataHelper Roles { get; }

    public TriggerDataHelper Triggers { get; }

    public FunDataHelper Fun { get; }

    public PetsDataHelper Pets { get; }

    public DataHelpers(StatsDataHelper statsHelper,
        ConfigDataHelper configHelper,
        RolesDataHelper rolesHelper,
        TriggerDataHelper triggersDataHelper,
        FunDataHelper fun,
        PetsDataHelper pets)
    {
        Stats = statsHelper;
        Config = configHelper;
        Roles = rolesHelper;
        Triggers = triggersDataHelper;
        Fun = fun;
        Pets = pets;
    }
}