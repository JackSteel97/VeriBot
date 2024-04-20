using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeriBot.Channels.SelfRole;
using VeriBot.Database.Models;
using VeriBot.DataProviders.SubProviders;
using VeriBot.DiscordModules.SelfRoles.Helpers;
using VeriBot.Responders;
using VeriBot.Services;

namespace VeriBot.DiscordModules.SelfRoles.Services;

public class SelfRoleMembershipService
{
    private readonly ErrorHandlingService _errorHandlingService;
    private readonly ILogger<SelfRoleMembershipService> _logger;
    private readonly SelfRolesProvider _selfRolesProvider;
    private readonly GuildsProvider _guildsProvider;

    public SelfRoleMembershipService(ErrorHandlingService errorHandlingService, SelfRolesProvider selfRolesProvider, ILogger<SelfRoleMembershipService> logger, GuildsProvider guildsProvider)
    {
        _errorHandlingService = errorHandlingService;
        _selfRolesProvider = selfRolesProvider;
        _logger = logger;
        _guildsProvider = guildsProvider;
    }

    public Task HandleReactionToggled(DiscordGuild guild, DiscordChannel channel, DiscordMessage message, DiscordEmoji emoji, DiscordUser user)
    {
        try
        {
            return HandleReactionToggledInternal(guild, channel, message, emoji, user);
        }
        catch (Exception e)
        {
            return _errorHandlingService.Log(e, "HandleReactionToggled - SelfRoles");
        }
    }

    public async Task Join(SelfRoleManagementAction request)
    {
        if (request.Action != SelfRoleActionType.Join) throw new ArgumentException($"Unexpected management action send to {nameof(Join)}");

        _logger.LogInformation("Request for user {UserId} to join self role {RoleName} in Guild {GuildId} received", request.Member.Id, request.RoleName, request.Member.Guild.Id);

        if (ValidateRequest(request, out var discordRole)
            && ValidateUserDoesNotAlreadyHaveRole(request.Member, discordRole.Id, discordRole.Mention, request.Responder))
        {
            await JoinRole(request.Member, discordRole);
            request.Responder.Respond(SelfRoleMessages.JoinedRoleSuccess(request.Member.Mention, discordRole.Mention));
        }
    }

    public async Task JoinAll(SelfRoleManagementAction request)
    {
        if (request.Action != SelfRoleActionType.JoinAll) throw new ArgumentException($"Unexpected management action send to {nameof(JoinAll)}");

        _logger.LogInformation("Request for user {UserId} to join All self roles in Guild {GuildId} received", request.Member.Id, request.Member.Guild.Id);

        if (_selfRolesProvider.TryGetGuildRoles(request.Member.Guild.Id, out var allSelfRoles))
        {
            var joinedRolesBuilder = await JoinRoles(request, allSelfRoles);
            if (joinedRolesBuilder.Length > 0)
                request.Responder.Respond(SelfRoleMessages.JoinedRolesSuccess(joinedRolesBuilder));
            else
                request.Responder.Respond(SelfRoleMessages.NoSelfRolesLeftToJoin());
        }
    }

    public async Task Leave(SelfRoleManagementAction request)
    {
        if (request.Action != SelfRoleActionType.Leave) throw new ArgumentException($"Unexpected management action send to {nameof(Leave)}");

        if (ValidateRequest(request, out var discordRole))
        {
            await LeaveRole(request.Member, discordRole);
            request.Responder.Respond(SelfRoleMessages.LeftRoleSuccess(request.Member.Mention, discordRole.Mention));
        }
    }
    
    private async Task HandleReactionToggledInternal(DiscordGuild guild, DiscordChannel channel, DiscordMessage message, DiscordEmoji emoji, DiscordUser user)
    {
        var selfRoleAssignmentMessage = _guildsProvider.GetGuildSelfRoleAssignmentMessageId(guild.Id);
        if (!selfRoleAssignmentMessage.message.HasValue)
        {
            _logger.LogInformation("Skipping handling reaction as there is no self roles assignment message for this guild");
            return;
        }
        if (channel.Id != selfRoleAssignmentMessage.channel || message.Id != selfRoleAssignmentMessage.message)
        {
            _logger.LogInformation("Skipping handling reaction as it is not a reaction on the guild's self roles assignment message");
            return;
        }

        if (!_selfRolesProvider.TryGetGuildRoles(guild.Id, out var allRoles))
        {
            _logger.LogInformation("Skipping handling reaction there are no self roles to check");
            return;
        }

        bool isAValidSelfRoleReaction = false;
        foreach (var role in allRoles)
        {
            if (role.EmojiId.HasValue && role.EmojiId.Value == emoji.Id)
            {
                isAValidSelfRoleReaction = true;
                await ToggleMembership((DiscordMember)user, role);
                break;
            }
        }

        if (!isAValidSelfRoleReaction)
        {
            await message.DeleteReactionAsync(emoji, user, "Not a valid self role reaction");
        }
    }

    private async Task ToggleMembership(DiscordMember member, SelfRole role)
    {
        var discordRole = member.Guild.Roles.Values.FirstOrDefault(discordRole => discordRole.Id == role.DiscordRoleId);
        if (member.Roles.Any(r => r.Id == role.DiscordRoleId))
        {
            await LeaveRole(member, discordRole);
        }
        else
        {
            await JoinRole(member, discordRole);
        }
    }

    private bool ValidateRequest(SelfRoleManagementAction request, out DiscordRole discordRole)
    {
        bool valid = true;
        discordRole = request.Member.Guild.Roles.Values.FirstOrDefault(role => role.Name.Equals(request.RoleName, StringComparison.OrdinalIgnoreCase));
        if (discordRole == default)
        {
            request.Responder.Respond(SelfRoleMessages.RoleDoesNotExist(request.RoleName));
            valid = false;
        }
        else if (!_selfRolesProvider.BotKnowsRole(request.Member.Guild.Id, discordRole.Id))
        {
            request.Responder.Respond(SelfRoleMessages.InvalidRole(discordRole.Mention));
            valid = false;
        }

        return valid;
    }

    private bool ValidateUserDoesNotAlreadyHaveRole(DiscordMember member, ulong discordRoleId, string roleMention, IResponder responder)
    {
        if (member.Roles.Any(r => r.Id == discordRoleId))
        {
            responder.Respond(SelfRoleMessages.AlreadyHasRole(roleMention));
            return false;
        }

        return true;
    }

    private async Task JoinRole(DiscordMember member, DiscordRole role, string reason = "Requested to join Self Role")
    {
        _logger.LogInformation("User {UserId} joining role {RoleName} in Guild {GuildId}", member.Id, role.Name, member.Guild.Id);
        await member.GrantRoleAsync(role, reason);
    }

    private async Task<StringBuilder> JoinRoles(SelfRoleManagementAction request, List<SelfRole> allSelfRoles)
    {
        var builder = new StringBuilder();
        foreach (var selfRole in allSelfRoles)
        {
            var discordRole = request.Member.Guild.GetRole(selfRole.DiscordRoleId);
            if (discordRole != default)
            {
                await JoinRole(request.Member, discordRole, "Requested to join All Self Roles");
                string roleMention = discordRole.IsMentionable ? discordRole.Mention : discordRole.Name;
                builder.AppendLine(Formatter.Bold(roleMention));
            }
            else
            {
                builder.Append(Formatter.Bold(selfRole.RoleName)).AppendLine(" - is not a valid role on this server make sure the server role has not been deleted");
            }
        }

        return builder;
    }

    private async Task LeaveRole(DiscordMember member, DiscordRole role, string reason = "Requested to leave Self Role")
    {
        _logger.LogInformation("User {UserId} leaving role {RoleName} in Guild {GuildId}", member.Id, role.Name, member.Guild.Id);
        await member.RevokeRoleAsync(role, reason);
    }
}