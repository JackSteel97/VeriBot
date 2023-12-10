﻿using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Text;
using VeriBot.Helpers;

namespace VeriBot.DiscordModules.SelfRoles.Helpers;

public static class SelfRoleMessages
{
    public static DiscordMessageBuilder NoSelfRolesAvailable() =>
        new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Warning($"There are no self roles available.{Environment.NewLine}Ask your administrator to create some!"));

    public static DiscordMessageBuilder NoSelfRolesLeftToJoin() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Warning("There are no roles that you don't already have."));

    public static DiscordMessageBuilder RoleDoesNotExist(string roleName) =>
        new DiscordMessageBuilder().WithEmbed(
            EmbedGenerator.Error($"{Formatter.Bold(roleName)} is not a valid role on this server.{Environment.NewLine}Make sure your administrator has added the role."));

    public static DiscordMessageBuilder RoleNameTooLong() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("The role name must be 255 characters or less."));

    public static DiscordMessageBuilder NoRoleNameProvided() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("No valid role name provided."));

    public static DiscordMessageBuilder RoleDescriptionTooLong() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("The role description must be 255 characters or less."));

    public static DiscordMessageBuilder NoRoleDescriptionProvided() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("No valid description provided."));

    public static DiscordMessageBuilder RoleNotCreatedYet() => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error("You must create the role in the server first."));

    public static DiscordMessageBuilder RoleAlreadyExists(string roleName) =>
        new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error($"The self role {Formatter.Bold(roleName)} already exists.{Environment.NewLine}Delete it first if you want to change it."));

    public static DiscordMessageBuilder RoleCreatedSuccess(string role) => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Success($"Self Role {role} created!"));

    public static DiscordMessageBuilder RoleRemovedSuccess(string roleName) => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Success($"Self Role {roleName} deleted!"));

    public static DiscordMessageBuilder InvalidRole(string role, bool deletedReminder = false)
    {
        const string deletedReminderMessage = "\n**Make sure the self role has not been removed.**";
        return new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Error($"{role} is not a valid self role.{(deletedReminder ? deletedReminderMessage : string.Empty)}"));
    }

    public static DiscordMessageBuilder AlreadyHasRole(string role) => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Warning($"You already have the {role} role"));

    public static DiscordMessageBuilder JoinedRoleSuccess(string user, string role) => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Success($"{user} joined {role}"));

    public static DiscordMessageBuilder JoinedRolesSuccess(StringBuilder joinedRoles) => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Success(joinedRoles.ToString(), "Joined Roles"));

    public static DiscordMessageBuilder LeftRoleSuccess(string user, string role) => new DiscordMessageBuilder().WithEmbed(EmbedGenerator.Success($"{user} left {role}"));
}