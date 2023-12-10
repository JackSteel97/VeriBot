﻿using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeriBot.Database.Models;
using VeriBot.Database.Models.Pets;
using VeriBot.Database.Models.Users;
using VeriBot.DiscordModules.Pets.Enums;
using VeriBot.DiscordModules.Pets.Generation;
using VeriBot.DiscordModules.Pets.Models;
using VeriBot.Helpers;
using VeriBot.Helpers.Extensions;
using VeriBot.Helpers.Levelling;
using VeriBot.Helpers.Maths;

namespace VeriBot.DiscordModules.Pets.Helpers;

public static class PetShared
{
    private const int _newPetSlotUnlockLevels = 20;
    private const int _startingPetSlots = 3;

    public static DiscordEmbedBuilder GetOwnedPetsBaseEmbed(User user, List<Pet> allPets, bool hasDisabledPets, string username = "Your")
    {
        var embedBuilder = new DiscordEmbedBuilder()
            .WithColor(EmbedGenerator.InfoColour)
            .WithTitle($"{username} Owned Pets");

        if (hasDisabledPets) embedBuilder.WithFooter("Inactive pet's bonuses have no effect until you reach the required level in this server or activate bonus pet slots.");

        int petCapacity = GetPetCapacity(user, allPets);
        embedBuilder
            .AddField("Pet Slots", $"{allPets.Count} / {petCapacity}")
            .WithTimestamp(DateTime.Now);

        return embedBuilder;
    }

    public static StringBuilder AppendPetDisplayShort(StringBuilder builder, Pet pet, bool active, double baseCapacity, double maxCapacity)
    {
        builder.Append(Formatter.Bold((pet.Priority + 1).Ordinalize())).Append(" - ").AppendLine(Formatter.Bold(pet.GetName().ToZalgo(pet.IsCorrupt)))
            .Append("Level ").Append(pet.CurrentLevel).Append(' ').Append(Formatter.Italic(pet.Rarity.ToString().ToZalgo(pet.IsCorrupt))).Append(' ')
            .Append(pet.Species.GetName().ToZalgo(pet.IsCorrupt));

        if (!active)
        {
            int levelRequired = GetRequiredLevelForPet(pet.Priority, baseCapacity, maxCapacity);
            builder.Append(" - **Inactive**, Level ").Append(levelRequired).Append(" required");
        }

        return builder.AppendLine()
            .AppendLine(GetPetLevelProgressBar(pet));
    }

    public static List<Pet> GetAvailablePets(User user, List<Pet> allPets, out List<Pet> disabledPets)
    {
        var availablePets = new List<Pet>();
        disabledPets = new List<Pet>();
        if (allPets.Count > 0)
        {
            int capacity = GetPetCapacity(user, allPets);
            var orderedPets = allPets.OrderBy(x => x.Priority).ToList();

            if (orderedPets.Count > capacity)
            {
                availablePets = orderedPets.GetRange(0, capacity);
                disabledPets = orderedPets.GetRange(capacity, orderedPets.Count - capacity);
            }
            else
            {
                availablePets = orderedPets;
            }
        }

        return availablePets;
    }

    public static int GetPetCapacity(User user, List<Pet> allPets)
    {
        int baseCapacity = GetBasePetCapacity(user);
        int bonusCapacity = (int)Math.Round(GetBonusValue(allPets, BonusType.PetSlots));
        int cappedBonusCapacity = Math.Min(50, bonusCapacity); // Cap at +50.
        int summedCapacity = baseCapacity + cappedBonusCapacity;
        int actualCapacity = Math.Max(1, summedCapacity); // Can't go less than 1 slot.
        return actualCapacity;
    }

    public static int GetBasePetCapacity(User user)
    {
        int result = _startingPetSlots;

        if (user != default) result += user.CurrentLevel / _newPetSlotUnlockLevels;
        return result;
    }

    public static bool TryGetPetIdFromComponentId(string buttonId, out long petId)
    {
        string[] parts = buttonId.Split(':');
        if (parts.Length == 2) return long.TryParse(parts[1], out petId);
        petId = default;
        return false;
    }

    public static bool PetXpChanged(Pet pet, StringBuilder changes, int levelOfUser, out bool shouldPingOwner)
    {
        shouldPingOwner = false;
        var newBonuses = new List<PetBonus>();
        bool levelledUp = LevellingMaths.UpdatePetLevel(pet.CurrentLevel, pet.EarnedXp, pet.Rarity, pet.IsCorrupt, out int newLevel);
        if (levelledUp)
        {
            changes.Append("Your pet ").Append(Formatter.Italic(pet.GetName())).Append(" advanced to level ").Append(Formatter.Bold(newLevel.ToString())).AppendLine(" and improved their abilities!");
            int nextLevel = pet.CurrentLevel + 1;
            pet.CurrentLevel = newLevel;
            for (int level = nextLevel; level <= pet.CurrentLevel; ++level)
            {
                var newBonus = PetLevelledUp(pet, level, levelOfUser);
                if (newBonus != null) newBonuses.Add(newBonus);
            }
        }

        if (newBonuses.Count > 0)
        {
            shouldPingOwner = true;
            changes.AppendLine(Formatter.Bold("Learned New Bonuses:"));
            foreach (var newBonus in newBonuses) PetDisplayHelpers.AppendBonusDisplay(changes, newBonus);
        }

        return levelledUp;
    }

    public static async Task SendPetLevelledUpMessage(StringBuilder changes, Guild guild, DiscordGuild discordGuild, User user, bool pingUser)
    {
        var channel = guild.GetLevelAnnouncementChannel(discordGuild);
        if (channel != null)
        {
            string content = "";
            if (pingUser && !user.OptedOutOfMentions)
                content = user.DiscordId.ToUserMention();
            else
                changes.Insert(0, user.DiscordId.ToUserMention() + Environment.NewLine);

            var embed = new DiscordEmbedBuilder(EmbedGenerator.Info(changes.ToString(), "Pet Level Up"));

            await channel.SendMessageAsync(content, embed);
        }
    }

    public static string GetPetLevelProgressBar(Pet pet)
    {
        const string progressCharacter = EmojiConstants.Symbols.GreenSquare;
        const char remainingCharacter = '⬛';

        double thisLevelXp = LevellingMaths.PetXpForLevel(pet.CurrentLevel, pet.Rarity, pet.IsCorrupt);
        double nextLevelXp = LevellingMaths.PetXpForLevel(pet.CurrentLevel + 1, pet.Rarity, pet.IsCorrupt);
        double xpIntoThisLevel = pet.EarnedXp - thisLevelXp;
        double xpToLevelUp = nextLevelXp - thisLevelXp;

        double progress = Math.Min(Math.Max(0, xpIntoThisLevel / xpToLevelUp), 1);
        const int totalCharacterCount = 10;
        int progressedCharacterCount = Convert.ToInt32(totalCharacterCount * progress);
        int remainingCharacters = totalCharacterCount - progressedCharacterCount;

        var sb = new StringBuilder();
        sb.Append('[');
        if (progressedCharacterCount > 0)
            for (int i = 0; i < progressedCharacterCount; ++i)
                sb.Append(progressCharacter);

        if (remainingCharacters > 0)
            for (int i = 0; i < remainingCharacters; ++i)
                sb.Append(remainingCharacter);
        sb.Append(']').Append(" Level ").Append(pet.CurrentLevel + 1);
        return sb.ToString();
    }

    public static double GetBonusValue(List<Pet> activePets, BonusType targetType)
    {
        bool isPercentage = targetType.IsPercentage();
        bool isRounded = targetType.IsRounded();
        double multiplier = isPercentage ? 1 : 0;
        foreach (var pet in activePets)
            foreach (var bonus in pet.Bonuses)
                if (bonus.BonusType.HasFlag(targetType))
                {
                    double value = bonus.Value;
                    if (isRounded) value = Math.Round(bonus.Value);
                    multiplier += value;
                }

        return multiplier;
    }

    public static List<PetWithActivation> Recombine(List<Pet> availablePets, List<Pet> disabledPets)
    {
        var combinedPets = new List<PetWithActivation>(availablePets.Count + disabledPets.Count);
        if (availablePets.Count > 0) combinedPets.AddRange(availablePets.ConvertAll(p => new PetWithActivation(p, true)));
        if (disabledPets.Count > 0) combinedPets.AddRange(disabledPets.ConvertAll(p => new PetWithActivation(p, false)));
        return combinedPets;
    }

    public static int GetRequiredLevelForPet(int petPriority, double baseCapacity, double totalCapacity)
    {
        double currentLevelBracket = (baseCapacity - _startingPetSlots) * _newPetSlotUnlockLevels;
        double extraRequiredLevels = (petPriority + 1 - totalCapacity) * _newPetSlotUnlockLevels;
        return Convert.ToInt32(currentLevelBracket + extraRequiredLevels);
    }

    private static PetBonus PetLevelledUp(Pet pet, int level, int levelOfUser)
    {
        PetBonus newBonus = null;
        double chanceToGenerateNewBonus = ((double)pet.Rarity + 1) / 10;
        if (level % 10 == 0 && MathsHelper.TrueWithProbability(chanceToGenerateNewBonus)) newBonus = GivePetNewBonus(pet, levelOfUser);
        ImproveCurrentPetBonuses(pet);
        return newBonus;
    }

    private static PetBonus GivePetNewBonus(Pet pet, int levelOfUser)
    {
        var bonus = PetBonusFactory.Generate(pet, levelOfUser);
        pet.AddBonus(bonus);
        return bonus;
    }

    private static void ImproveCurrentPetBonuses(Pet pet)
    {
        foreach (var bonus in pet.Bonuses) bonus.LevelUp();
    }
}