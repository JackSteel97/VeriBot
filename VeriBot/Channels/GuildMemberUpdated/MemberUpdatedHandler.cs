using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeriBot.Services;

namespace VeriBot.Channels.GuildMemberUpdated;

public class MemberUpdatedHandler
{
    private readonly NotifierService _notifierService;
    private readonly ILogger<MemberUpdatedHandler> _logger;
    private static readonly HashSet<ulong> _triggerRoleIds = new() { 1136781629948428369, 1161704036244930661, 1289155977471463468  };
    private const ulong _newRoleId = 1137369187652747295;
    
    public MemberUpdatedHandler(NotifierService notifierService, ILogger<MemberUpdatedHandler> logger)
    {
        _notifierService = notifierService;
        _logger = logger;
    }

    public async ValueTask HandleUpdate(GuildMemberUpdateEventArgs args)
    {
        _logger.LogInformation("Handling member update for {UserId} in {GuildId}", args.Member.Id, args.Guild.Id);

        await RevokeRolesBasedOnCurrentAssignedRoles(args);
        await GrantRolesBasedOnAnyNewlyAssignedRoles(args);
        
        _logger.LogInformation("Successfully handled member update for {UserId} in {GuildId}", args.Member.Id, args.Guild.Id);
    }

    private async ValueTask GrantRolesBasedOnAnyNewlyAssignedRoles(GuildMemberUpdateEventArgs args)
    {
        bool alreadyHasNewRole = args.MemberAfter.Roles.Any(x => x.Id == _newRoleId);
        if (alreadyHasNewRole)
        {
            _logger.LogInformation("User {UserId} already has new role {RoleId} so there is nothing more to do", args.Member.Id, _newRoleId);
            return;
        }
        
        var triggerRole = args.MemberAfter.Roles.FirstOrDefault(x => _triggerRoleIds.Contains(x.Id));
        if (triggerRole == default)
        {
            _logger.LogInformation("This update did not grant the user {UserId} any target role, there is nothing more to do", args.Member.Id);
            return;
        }

        var roleToGrant = args.Guild.GetRole(_newRoleId);
        if (roleToGrant == null)
        {
            throw new InvalidOperationException($"The role {_newRoleId} could not be retrieved from Guild {args.Guild.Id} to grant to User {args.Member.Id}");
        }
        
        _logger.LogInformation("User {UserId} has been granted the trigger role {TriggerRoleId} and will be granted the new role {NewRoleId}", args.Member.Id, triggerRole.Id, _newRoleId);
        await args.Member.GrantRoleAsync(roleToGrant, $"Was granted the {triggerRole.Name} role");
        await _notifierService.SendRoleGrantedMessageToHaslam(args.Member, triggerRole.Name, roleToGrant.Name);
    }
    
    private async ValueTask RevokeRolesBasedOnCurrentAssignedRoles(GuildMemberUpdateEventArgs args)
    {
        bool hasNewRole = args.MemberAfter.Roles.Any(x => x.Id == _newRoleId);
        if (!hasNewRole)
        {
            _logger.LogInformation("User {UserId} does not have the new role {RoleId} so there is nothing more to do", args.Member.Id, _newRoleId);
            return;
        }
        
        var triggerRole = args.MemberAfter.Roles.FirstOrDefault(x => _triggerRoleIds.Contains(x.Id));
        if (triggerRole != default)
        {
            _logger.LogInformation("User {UserId} has the required trigger role, there is nothing more to do", args.Member.Id);
            return;
        }

        var roleToRevoke = args.Guild.GetRole(_newRoleId);
        if (roleToRevoke == null)
        {
            throw new InvalidOperationException($"The role {_newRoleId} could not be retrieved from Guild {args.Guild.Id} to revoke from User {args.Member.Id}");
        }
        
        _logger.LogInformation("User {UserId} no longer has any trigger role will have the new role {NewRoleId} revoked", args.Member.Id, _newRoleId);
        await args.Member.RevokeRoleAsync(roleToRevoke, "No longer has any of the required roles for this role");
        await _notifierService.SendRoleRevokedMessageToHaslam(args.Member, roleToRevoke.Name);
    }
}