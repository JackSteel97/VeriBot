﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using VeriBot.Database.Models.Pets;
using VeriBot.DiscordModules.Pets.Enums;
using VeriBot.Helpers.Maths;

namespace VeriBot.DiscordModules.Pets.Generation;

public class PetFactory
{
    private readonly ILogger<PetFactory> _logger;

    private readonly Dictionary<Rarity, List<Species>> _speciesByRarity = new();

    public PetFactory(ILogger<PetFactory> logger)
    {
        _logger = logger;
        BuildSpeciesCache();
    }

    private void BuildSpeciesCache()
    {
        var species = Enum.GetValues(typeof(Species)).Cast<Species>().ToArray();
        foreach (var spec in species)
        {
            var rarity = spec.GetRarity();
            if (!_speciesByRarity.TryGetValue(rarity, out var includedSpecies))
            {
                includedSpecies = new List<Species>();
                _speciesByRarity.Add(rarity, includedSpecies);
            }

            includedSpecies.Add(spec);
        }
    }

    public Pet Generate(int levelOfUser = 0)
    {
        var baseRarity = GetBaseRarity(levelOfUser);
        var species = GetSpecies(baseRarity);
        var finalRarity = GetFinalRarity(baseRarity, levelOfUser);
        var size = PetGenerationShared.GetRandomEnumValue<Size>();
        var birthDate = GetBirthDate(species);

        var pet = new Pet
        {
            Rarity = finalRarity,
            Species = species,
            Size = size,
            BornAt = birthDate,
            FoundAt = DateTime.UtcNow
        };

        pet.Attributes = BuildAttributes(pet);

        var bonuses = PetBonusFactory.GenerateMany(pet, levelOfUser);
        pet.AddBonuses(bonuses);

        _logger.LogDebug("Generated a new pet with Base Rarity {BaseRarity} and Final Rarity {FinalRarity}", baseRarity.ToString(), finalRarity.ToString());
        return pet;
    }

    private static Rarity GetBaseRarity(int userLevel)
    {
        const int maxBound = 100000;
        const double MythicalChance = 0.0001;
        const double LegendaryChance = 0.005;
        const double EpicChance = 0.06;
        const double RareChance = 0.20;
        const double UncommonChance = 0.50;

        double scaler = GetRarityScaler(userLevel);
        double mythicalChanceScaled = ScaleChance(MythicalChance, scaler);
        double legendaryChanceScaled = ScaleChance(LegendaryChance, scaler);
        double epicChanceScaled = ScaleChance(EpicChance, scaler);
        double rareChanceScaled = ScaleChance(RareChance, scaler);

        double mythicalBound = maxBound * mythicalChanceScaled;
        double legendaryBound = maxBound * legendaryChanceScaled;
        double epicBound = maxBound * epicChanceScaled;
        double rareBound = maxBound * rareChanceScaled;
        const double UncommonBound = maxBound * UncommonChance;

        int random = RandomNumberGenerator.GetInt32(maxBound);

        if (random <= mythicalBound) return Rarity.Mythical;

        if (random <= legendaryBound) return Rarity.Legendary;

        if (random <= epicBound) return Rarity.Epic;

        if (random <= rareBound) return Rarity.Rare;

        return random <= UncommonBound ? Rarity.Uncommon : Rarity.Common;
    }

    private static double ScaleChance(double chance, double scaler) => chance + chance * scaler;

    private static double GetRarityScaler(int userLevel) =>
        userLevel switch
        {
            > 100 => 0.5,
            > 80 => 0.4,
            > 60 => 0.3,
            > 40 => 0.2,
            _ => 0
        };

    private static Rarity GetFinalRarity(Rarity rarity, int levelOfUser)
    {
        const double baseChance = 0.1;
        const double levelDivisor = 50;
        double rarityUpChance = baseChance * Math.Max(1, levelOfUser / levelDivisor);
        const int maxRarity = (int)Rarity.Mythical;
        int currentRarity = (int)rarity;

        int finalRarity = currentRarity;

        for (int i = currentRarity + 1; i <= maxRarity; ++i)
        {
            if (!MathsHelper.TrueWithProbability(rarityUpChance)) break;

            finalRarity = i;
        }

        return (Rarity)finalRarity;
    }

    private Species GetSpecies(Rarity rarity)
    {
        var possibleSpecies = _speciesByRarity[rarity];
        int index = RandomNumberGenerator.GetInt32(possibleSpecies.Count);
        return possibleSpecies[index];
    }

    private static DateTime GetBirthDate(Species species)
    {
        int maxAgeMinutes = (int)Math.Floor(species.GetMaxStartingAge().TotalMinutes);
        int minutesOld = RandomNumberGenerator.GetInt32(30, maxAgeMinutes);
        return DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(minutesOld));
    }

    private static List<PetAttribute> BuildAttributes(Pet pet)
    {
        var bodyParts = pet.Species.GetBodyParts();

        var attributes = new List<PetAttribute>(bodyParts.Count);
        foreach (var part in bodyParts)
        {
            var attribute = new PetAttribute { Pet = pet, Name = part.ToString(), Description = GenerateColourCombo() };
            attributes.Add(attribute);
        }

        return attributes;
    }

    private static string GenerateColourCombo()
    {
        if (MathsHelper.TrueWithProbability(0.1))
        {
            // Has two colours.
            var primary = PetGenerationShared.GetRandomEnumValue<Colour>();
            var secondary = PetGenerationShared.GetRandomEnumValue<Colour>();
            var mixing = PetGenerationShared.GetRandomEnumValue<ColourMixing>();

            return $"{primary} and {secondary} {mixing} patterned";
        }

        // Has one colour.
        var colour = PetGenerationShared.GetRandomEnumValue<Colour>();
        return colour.ToString();
    }
}