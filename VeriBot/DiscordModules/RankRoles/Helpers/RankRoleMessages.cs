﻿using DSharpPlus;
using DSharpPlus.Entities;
using VeriBot.Helpers;

namespace VeriBot.DiscordModules.RankRoles.Helpers;

public static class RankRoleMessages
{
    public static DiscordMessageBuilder NoRankRolesForThisServer() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Warning("There are no Rank Roles currently set up for this server."));

    public static DiscordMessageBuilder RoleNameTooLong() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("The role name must be 255 characters or less."));

    public static DiscordMessageBuilder NoRoleNameProvided() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("No valid role name provided."));

    public static DiscordMessageBuilder RequiredRankMustBePositive() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("The required rank must be positive."));

    public static DiscordMessageBuilder RoleAlreadyExists() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("This rank role already exists, please delete the existing role first."));

    public static DiscordMessageBuilder RoleAlreadyExistsForLevel(int requiredRank, string existingRoleName) => new DiscordMessageBuilder().WithEmbed(
        EmbedGenerator.Error($"A rank role already exists for level {Formatter.InlineCode(requiredRank.ToString())} - {Formatter.Bold(existingRoleName)}, please delete the existing role."));

    public static DiscordMessageBuilder RoleDoesNotExistOnServer(string roleName) =>
        new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error($"The Role {Formatter.Bold(roleName)} does not exist. You must create the role in the server first."));

    public static DiscordMessageBuilder RankRoleCreatedSuccess(string roleName, int requiredRank, string alreadyAchievedUsers) => new DiscordMessageBuilder().WithEmbed(
        EmbedGenerator.Success($"{Formatter.Bold(roleName)} set as a Rank Role for Rank {Formatter.Bold(requiredRank.ToString())}\n\n{alreadyAchievedUsers}", "Rank Role Created!"));

    public static DiscordMessageBuilder RankRoleDeletedSuccess(string roleName) => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Success($"Rank Role {Formatter.Bold(roleName)} deleted!"));
}