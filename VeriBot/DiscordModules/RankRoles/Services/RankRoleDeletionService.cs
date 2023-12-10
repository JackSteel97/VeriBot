﻿using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeriBot.Channels.RankRole;
using VeriBot.Database.Models;
using VeriBot.Database.Models.Users;
using VeriBot.DataProviders.SubProviders;
using VeriBot.DiscordModules.RankRoles.Helpers;
using VeriBot.Services;

namespace VeriBot.DiscordModules.RankRoles.Services;

public class RankRoleDeletionService
{
    private readonly ErrorHandlingService _errorHandlingService;
    private readonly LevelMessageSender _levelMessageSender;
    private readonly ILogger<RankRoleDeletionService> _logger;
    private readonly RankRolesProvider _rankRolesProvider;
    private readonly UserLockingService _userLockingService;
    private readonly UsersProvider _usersProvider;

    public RankRoleDeletionService(ILogger<RankRoleDeletionService> logger,
        RankRolesProvider rankRolesProvider,
        UsersProvider usersProvider,
        UserLockingService userLockingService,
        LevelMessageSender levelMessageSender,
        ErrorHandlingService errorHandlingService)
    {
        _logger = logger;
        _rankRolesProvider = rankRolesProvider;
        _usersProvider = usersProvider;
        _userLockingService = userLockingService;
        _levelMessageSender = levelMessageSender;
        _errorHandlingService = errorHandlingService;
    }

    public async ValueTask Delete(RankRoleManagementAction request)
    {
        _logger.LogInformation("Request to delete Rank Role {RoleName} in Guild {GuildId} received", request.GetRoleIdentifier(), request.Guild.Id);
        await DeleteRole(request);
    }

    private async ValueTask DeleteRole(RankRoleManagementAction request)
    {
        if (TryGetRankRole(request, out var roleToDelete) && _rankRolesProvider.TryGetGuildRankRoles(request.Guild.Id, out var guildRoles) && guildRoles.Count > 0)
        {
            var excludedRoles = new HashSet<ulong> { roleToDelete.RoleDiscordId };

            await RemoveRoleFromUsers(request.Guild, roleToDelete, guildRoles, excludedRoles);

            await _rankRolesProvider.RemoveRole(request.Guild.Id, roleToDelete.RoleDiscordId);

            request.Responder.Respond(RankRoleMessages.RankRoleDeletedSuccess(roleToDelete.RoleName));
        }
        else
        {
            request.Responder.Respond(RankRoleMessages.RoleDoesNotExistOnServer(request.RoleName));
        }
    }

    private async Task RemoveRoleFromUsers(DiscordGuild guild, RankRole roleToDelete, List<RankRole> guildRoles, HashSet<ulong> excludedRoles)
    {
        using (await _userLockingService.WriteLockAllUsersAsync(guild.Id))
        {
            var guildUsers = _usersProvider.GetUsersInGuild(guild.Id);
            foreach (var guildUser in guildUsers) await HandleRoleRemovalFromUser(guild, roleToDelete, guildRoles, excludedRoles, guildUser);
        }
    }

    private async ValueTask HandleRoleRemovalFromUser(DiscordGuild guild, RankRole roleToDelete, List<RankRole> guildRoles, HashSet<ulong> excludedRoles, User guildUser)
    {
        // Do we need to remove this role from this user?
        if (guildUser.CurrentRankRoleRowId == roleToDelete.RowId)
        {
            var member = await guild.GetMemberAsync(guildUser.DiscordId);
            if (member == null)
            {
                _logger.LogWarning("Attempt to handle the removal of the Role {RoleId} from User {UserId} in Guild {GuildId} could not complete because the member data could not be retrieved",
                    roleToDelete.RoleDiscordId, guildUser.DiscordId, guild.Id);
                return;
            }

            // Remove their old, about to be deleted role.
            await RemoveRoleFromUser(guild, roleToDelete.RoleDiscordId, member, "This rank role was deleted by an admin");

            // Find a role to replace it with and replace it if possible.
            var roleToGrant = await ReplaceRoleWithNext(guild, guildRoles, excludedRoles, guildUser, member);

            // Update their role and notify the user.
            await _usersProvider.UpdateRankRole(guild.Id, guildUser.DiscordId, roleToGrant);
            _levelMessageSender.SendRankChangeDueToDeletionMessage(guild, guildUser.DiscordId, roleToDelete, roleToGrant?.RoleDiscordId);
        }
    }

    private async ValueTask<RankRole> ReplaceRoleWithNext(DiscordGuild guild, List<RankRole> guildRoles, HashSet<ulong> excludedRoles, User guildUser, DiscordMember member)
    {
        var roleToGrant = RankRoleShared.FindHighestRankRoleForLevel(guildRoles, guildUser.CurrentLevel, guildUser.CurrentRankRole, excludedRoles, true);

        // If their role can be replaced, replace it
        if (roleToGrant != default) await GrantRoleToUser(guild, roleToGrant.RoleDiscordId, member, "Previous rank role deleted");

        return roleToGrant;
    }

    private bool TryGetRankRole(RankRoleManagementAction request, out RankRole role)
    {
        if (request.RoleId != default) return _rankRolesProvider.TryGetRole(request.Guild.Id, request.RoleId, out role);

        // Fallback to search by name.
        if (_rankRolesProvider.TryGetGuildRankRoles(request.Guild.Id, out var allGuildRoles))
        {
            role = allGuildRoles.Find(x => x.RoleName.Equals(request.RoleName, StringComparison.OrdinalIgnoreCase));
            return role != default;
        }

        role = default;
        return false;
    }

    private async ValueTask RemoveRoleFromUser(DiscordGuild guild, ulong roleId, DiscordMember member, string reason)
    {
        var role = guild.GetRole(roleId);
        if (role == null)
        {
            _logger.LogWarning("Attempt to remove role {RoleId} from User {UserId} in Guild {GuildId} could not complete because the role does not exist", roleId, member.Id, guild.Id);
            return;
        }

        await member.RevokeRoleAsync(role, reason);
    }

    private async ValueTask GrantRoleToUser(DiscordGuild guild, ulong roleId, DiscordMember member, string reason)
    {
        var role = guild.GetRole(roleId);
        if (role == null)
        {
            _logger.LogWarning("Attempt to grant role {RoleId} to User {UserId} in Guild {GuildId} could not complete because the role does not exist", roleId, member.Id, guild.Id);
            return;
        }

        await member.GrantRoleAsync(role, reason);
    }
}