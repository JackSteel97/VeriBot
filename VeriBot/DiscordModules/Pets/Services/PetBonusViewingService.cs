﻿using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using VeriBot.Channels.Pets;
using VeriBot.Database.Models.Users;
using VeriBot.DataProviders;
using VeriBot.DiscordModules.Pets.Helpers;
using VeriBot.DiscordModules.Pets.Models;

namespace VeriBot.DiscordModules.Pets.Services;

public class PetBonusViewingService
{
    private readonly DataCache _cache;

    public PetBonusViewingService(DataCache cache)
    {
        _cache = cache;
    }

    public void View(PetCommandAction request)
    {
        if (request.Action != PetCommandActionType.ViewBonuses) throw new ArgumentException($"Unexpected action type sent to {nameof(View)}");

        ViewPetBonuses(request);
    }

    private void ViewPetBonuses(PetCommandAction request)
    {
        if (!_cache.Users.TryGetUser(request.Guild.Id, request.Target.Id, out var user)
            || !_cache.Pets.TryGetUsersPets(request.Target.Id, out var pets))
        {
            request.Responder.Respond(PetMessages.GetNoPetsAvailableMessage());
            return;
        }

        var availablePets = PetShared.GetAvailablePets(user, pets, out var disabledPets);
        var combinedPets = PetShared.Recombine(availablePets, disabledPets);
        if (combinedPets.Count > 0)
        {
            var pages = BuildPages(user, request.Target, combinedPets);
            request.Responder.RespondPaginated(pages);
        }
    }

    private static List<Page> BuildPages(User user, DiscordMember member, List<PetWithActivation> allPets)
    {
        int maxCapacity = PetShared.GetPetCapacity(user, allPets.ConvertAll(p => p.Pet));
        int baseCapacity = PetShared.GetBasePetCapacity(user);
        var pages = PetDisplayHelpers.GetPetBonusesSummary(allPets, member.Username, member.AvatarUrl, baseCapacity, maxCapacity);

        return pages;
    }
}