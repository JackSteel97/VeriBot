﻿using DSharpPlus;
using DSharpPlus.Entities;
using System;
using VeriBot.Database.Models.Pets;
using VeriBot.DiscordModules.Pets.Enums;
using VeriBot.Helpers;

namespace VeriBot.DiscordModules.Pets.Helpers;

public static class PetMessages
{
    public static DiscordMessageBuilder GetPetRanAwayMessage(Pet pet)
    {
        var embedBuilder = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor(pet.Rarity.GetColour(pet.IsCorrupt)))
            .WithTitle("It got away!")
            .WithDescription($"The {pet.Species.GetName()} ran away before you could befriend it.{Environment.NewLine}Better luck next time!");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetPetDiedMessage(Pet pet)
    {
        var embedBuilder = new DiscordEmbedBuilder()
            .WithColor(EmbedGenerator.WarningColour)
            .WithTitle("Pet Retired!")
            .WithDescription($"Your {pet.Rarity.ToString()} {pet.Species.GetName()} \"{Formatter.Italic(pet.GetName())}\" has grown too old and left to retire peacefully.");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetBefriendFailedMessage(Pet pet)
    {
        var embedBuilder = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor(pet.Rarity.GetColour(pet.IsCorrupt)))
            .WithTitle("Failed to befriend!")
            .WithDescription($"The {pet.Species.GetName()} ran away as soon as you moved closer.{Environment.NewLine}Better luck next time!");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetPetCorruptedMessage(Pet pet)
    {
        var embed = PetDisplayHelpers.GetPetDisplayEmbed(pet, false);
        return new DiscordMessageBuilder().WithEmbed(embed).WithContent("Your new pet became Corrupted during the befriending process!!");
    }

    public static DiscordMessageBuilder GetNamingSuccessMessage(Pet pet)
    {
        var embedBuilder = EmbedGenerator.Success($"You named this pet {pet.Species.GetName()} {Formatter.Italic(pet.GetName())}");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetMakePrimarySuccessMessage(Pet pet)
    {
        var embedBuilder = EmbedGenerator.Success($"{Formatter.Bold(pet.GetName())} Is now your primary pet and will receive a much larger share of XP!");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetMoveToBottomSuccessMessage(Pet pet)
    {
        var embedBuilder = EmbedGenerator.Success($"{Formatter.Bold(pet.GetName())} has been moved to the bottom of your pet list and will receive a much smaller share of XP.");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetPriorityIncreaseSuccessMessage(Pet pet)
    {
        var embedBuilder = EmbedGenerator.Success($"{Formatter.Bold(pet.GetName())} has been moved up in your pet list and will receive a larger share of XP.");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetPriorityDecreaseSuccessMessage(Pet pet)
    {
        var embedBuilder = EmbedGenerator.Success($"{Formatter.Bold(pet.GetName())} has been moved down in your pet list and will receive a smaller share of XP.");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetAbandonSuccessMessage(Pet pet)
    {
        var embedBuilder = EmbedGenerator.Success($"{Formatter.Bold(pet.GetName())} has been released into the wild, freeing up a pet slot.");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetPetTreatedMessage(Pet pet, double xpGain)
    {
        var embedBuilder = EmbedGenerator.Info($"{Formatter.Bold(pet.GetName())} Greatly enjoyed their treat and gained {xpGain:N0} XP", "Tasty!");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }

    public static DiscordMessageBuilder GetNoPetsAvailableMessage()
    {
        var embedBuilder = EmbedGenerator.Info("You don't currently have any pets, use the `Pet Search` command to get some!", "No pets!");
        return new DiscordMessageBuilder().WithEmbed(embedBuilder);
    }
}