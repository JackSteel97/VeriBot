using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using VeriBot.Channels.Stats;
using VeriBot.DataProviders.SubProviders;
using VeriBot.DiscordModules.Stats.Helpers;
using VeriBot.Helpers;
using VeriBot.Services;

namespace VeriBot.DiscordModules.Stats.Services;

public class StatsCardService
{
    //private readonly LevelCardGenerator _levelCardGenerator;
    private readonly UserLockingService _userLockingService;
    private readonly UsersProvider _usersProvider;

    public StatsCardService(UserLockingService userLockingService, UsersProvider usersProvider /*, LevelCardGenerator levelCardGenerator*/)
    {
        _userLockingService = userLockingService;
        _usersProvider = usersProvider;
        //_levelCardGenerator = levelCardGenerator;
    }

    public async Task View(StatsCommandAction request)
    {
        if (request.Action != StatsCommandActionType.ViewPersonalStats) throw new ArgumentException($"Unexpected action type sent to {nameof(View)}");

        using (await _userLockingService.ReaderLockAsync(request.Guild.Id, request.Target.Id))
        {
            if (!_usersProvider.TryGetUser(request.Guild.Id, request.Target.Id, out var user))
            {
                request.Responder.Respond(StatsMessages.UnableToFindUser());
                return;
            }
            
            var message = StatsMessages.GetStatsEmbed(user, request.Target.Username);
            var msgBuilder = new DiscordMessageBuilder()
                .WithEmbed(message);
            request.Responder.Respond(msgBuilder);
            
            /*using (var imageStream = await _levelCardGenerator.GenerateCard(user, request.Target))
            {
                var message = StatsMessages.StatsCard(user, request.Target.Username, imageStream);
                request.Responder.Respond(message);
            }*/
        }
    }
}