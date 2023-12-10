using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VeriBot.Channels.Pets;
using VeriBot.Database.Models.Pets;
using VeriBot.DataProviders;
using VeriBot.DiscordModules.Pets.Enums;
using VeriBot.DiscordModules.Pets.Generation;
using VeriBot.DiscordModules.Pets.Helpers;
using VeriBot.Helpers;
using VeriBot.Helpers.Constants;
using VeriBot.Helpers.Extensions;
using VeriBot.Helpers.Maths;
using VeriBot.Responders;
using VeriBot.Services;

namespace VeriBot.DiscordModules.Pets.Services;

public class PetSearchingService
{
    private const string _replacementWarningMessage = "Warning: If you befriend this pet you must choose an existing one to release.";
    private readonly PetBefriendingService _befriendingService;
    private readonly DataCache _cache;
    private readonly ErrorHandlingService _errorHandlingService;
    private readonly ILogger<PetSearchingService> _logger;
    private readonly PetFactory _petFactory;

    public PetSearchingService(DataCache cache, PetFactory petFactory, ILogger<PetSearchingService> logger, ErrorHandlingService errorHandlingService, PetBefriendingService befriendingService)
    {
        _cache = cache;
        _petFactory = petFactory;
        _logger = logger;
        _errorHandlingService = errorHandlingService;
        _befriendingService = befriendingService;
    }

    public async Task Search(PetCommandAction request)
    {
        if (request.Action != PetCommandActionType.Search) throw new ArgumentException($"Unexpected action type sent to {nameof(Search)}");
        await SearchCore(request);
    }

    private async Task SearchCore(PetCommandAction request)
    {
        if (!CanSearch(request.Member, request.Responder, out bool isReplacementSearch)
            || !FoundAPet(request.Member, request.Responder))
            return;

        (bool befriend, var foundPet, var interaction) = await HandlePetFound(request, isReplacementSearch);
        if (befriend) await _befriendingService.Befriend(request, foundPet, interaction);
    }

    private bool CanSearch(DiscordMember member, IResponder responder, out bool isReplacementSearch)
    {
        isReplacementSearch = false;
        if (PetSpaceHelper.HasSpaceForAnotherPet(member, _cache.Users, _cache.Pets)) return true;

        isReplacementSearch = PetSpaceHelper.CanReplaceToBefriend(member, _cache.Users, _cache.Pets);
        if (!isReplacementSearch)
        {
            _logger.LogInformation("User {UserId} cannot search for a new pet because their pet slots are already full", member.Id);
            var response = new DiscordMessageBuilder()
                .WithEmbed(EmbedGenerator.Warning($"You don't have enough room for another pet{Environment.NewLine}Use `Pet Manage` to release one of your existing pets to make room"));
            responder.Respond(response);
            return false;
        }

        _logger.LogInformation("User {UserId} is performing a replacement search", member.Id);
        return true;
    }

    private bool FoundAPet(DiscordMember member, IResponder responder)
    {
        if (SearchSuccess(member)) return true;

        _logger.LogInformation("User {UserId} failed to find anything when searching for a new pet", member.Id);
        var response = new DiscordMessageBuilder()
            .WithEmbed(EmbedGenerator.Info($"You didn't find anything this time!{Environment.NewLine}Try again later", "Nothing Found"));
        responder.Respond(response);
        return false;
    }

    private async Task<(bool befriendAttempt, Pet foundPet, DiscordInteraction interaction)> HandlePetFound(PetCommandAction request, bool mustReplaceToBefriend)
    {
        if (!_cache.Users.TryGetUser(request.Guild.Id, request.Member.Id, out var user)) return (false, null, null);

        var foundPet = _petFactory.Generate(user.CurrentLevel);

        var response = BuildResponse(foundPet, mustReplaceToBefriend);
        var message = await request.Responder.RespondAsync(response);
        var result = await message.WaitForButtonAsync(request.Member);
        request.Responder.SetInteraction(result.Result?.Interaction);
        if (result.TimedOut)
        {
            _logger.LogInformation("Pet found message timed out waiting for a user response from User {UserId} in Guild {GuildId}", request.Member.Id, request.Guild.Id);
            message.DeleteAsync().FireAndForget(_errorHandlingService);
            return (false, null, null);
        }

        response.ClearComponents();
        message.ModifyAsync(response).FireAndForget(_errorHandlingService);
        return (result.Result.Id == InteractionIds.Pets.Befriend, foundPet, result.Result.Interaction);
    }

    private DiscordMessageBuilder BuildResponse(Pet foundPet, bool mustReplaceToBefriend)
    {
        var initialPetDisplay = PetDisplayHelpers.GetPetDisplayEmbed(foundPet, false);

        var initialResponseBuilder = new DiscordMessageBuilder()
            .WithContent($"You found a new potential friend!{(mustReplaceToBefriend ? string.Concat(Environment.NewLine, _replacementWarningMessage) : string.Empty)}")
            .WithEmbed(initialPetDisplay)
            .AddComponents(Interactions.Pets.Befriend, Interactions.Pets.Leave);
        return initialResponseBuilder;
    }

    private bool SearchSuccess(DiscordMember userSearching)
    {
        double probability = GetSearchSuccessProbability(userSearching);
        _logger.LogInformation("Search success probability for User {UserId} is {Probability}", userSearching.Id, probability);
        return MathsHelper.TrueWithProbability(probability);
    }

    private double GetSearchSuccessProbability(DiscordMember userSearching)
    {
        int ownedPetCount = 0;
        double bonusMultiplier = 1;
        if (_cache.Users.TryGetUser(userSearching.Guild.Id, userSearching.Id, out var user)
            && _cache.Pets.TryGetUsersPets(userSearching.Id, out var ownedPets))
        {
            var activePets = PetShared.GetAvailablePets(user, ownedPets, out _);
            ownedPetCount = ownedPets.Count;

            bonusMultiplier = PetShared.GetBonusValue(activePets, BonusType.SearchSuccessRate);
        }

        double probability = 2D / ownedPetCount * bonusMultiplier;
        double finalProbability = Math.Min(1, probability);
        return finalProbability;
    }
}