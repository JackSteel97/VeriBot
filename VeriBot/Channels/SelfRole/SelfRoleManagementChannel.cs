using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VeriBot.Channels;
using VeriBot.DiscordModules.SelfRoles.Services;
using VeriBot.Services;

namespace VeriBot.Channels.SelfRole;

public class SelfRoleManagementChannel : BaseChannel<SelfRoleManagementAction>
{
    private readonly SelfRoleCreationService _selfRoleCreationService;
    private readonly SelfRoleMembershipService _selfRoleMembershipService;

    public SelfRoleManagementChannel(ILogger<SelfRoleManagementChannel> logger,
        ErrorHandlingService errorHandlingService,
        SelfRoleCreationService selfRoleCreationService,
        SelfRoleMembershipService selfRoleMembershipService) : base(logger, errorHandlingService, "Self Role")
    {
        _selfRoleCreationService = selfRoleCreationService;
        _selfRoleMembershipService = selfRoleMembershipService;
    }

    protected override async ValueTask HandleMessage(SelfRoleManagementAction message)
    {
        switch (message.Action)
        {
            case SelfRoleActionType.Join:
                await _selfRoleMembershipService.Join(message);
                break;
            case SelfRoleActionType.JoinAll:
                await _selfRoleMembershipService.JoinAll(message);
                break;
            case SelfRoleActionType.Leave:
                await _selfRoleMembershipService.Leave(message);
                break;
            case SelfRoleActionType.Create:
                await _selfRoleCreationService.Create(message);
                break;
            case SelfRoleActionType.Delete:
                await _selfRoleCreationService.Remove(message);
                break;
        }
    }
}