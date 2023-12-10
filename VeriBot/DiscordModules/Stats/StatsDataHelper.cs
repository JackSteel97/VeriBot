using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VeriBot.DataProviders;
using VeriBot.DiscordModules.Pets;
using VeriBot.Helpers.Levelling;

namespace VeriBot.DiscordModules.Stats;

public class StatsDataHelper
{
    private readonly DataCache _cache;
    private readonly ILogger<StatsDataHelper> _logger;
    private readonly PetsDataHelper _petsDataHelper;

    public StatsDataHelper(DataCache cache,
        ILogger<StatsDataHelper> logger,
        PetsDataHelper petsDataHelper)
    {
        _cache = cache;
        _logger = logger;
        _petsDataHelper = petsDataHelper;
    }

    /// <summary>
    ///     Called during app shutdown to make sure no timings get carried too long during downtime.
    /// </summary>
    public async Task DisconnectAllUsers()
    {
        _logger.LogInformation("Disconnecting all users from voice stats");
        var allUsers = _cache.Users.GetAllUsers();

        foreach (var user in allUsers)
        {
            var copyOfUser = user.Clone();
            var availablePets = _petsDataHelper.GetAvailablePets(user.Guild.DiscordId, user.DiscordId, out _);

            // Pass null to reset all start times.
            copyOfUser.VoiceStateChange(null, availablePets, 1, true, false);
            copyOfUser.UpdateLevel();
            await _cache.Users.UpdateUser(user.Guild.DiscordId, copyOfUser);
            await _petsDataHelper.PetXpUpdated(availablePets, default, copyOfUser.CurrentLevel); // Default - Don't try to send level up messages
        }
    }
}