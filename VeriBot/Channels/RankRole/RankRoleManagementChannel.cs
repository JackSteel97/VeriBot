using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VeriBot.Channels;
using VeriBot.DiscordModules.RankRoles.Services;
using VeriBot.Services;

namespace VeriBot.Channels.RankRole;

public class RankRoleManagementChannel : BaseChannel<RankRoleManagementAction>
{
    private readonly RankRoleCreationService _rankRoleCreationService;
    private readonly RankRoleDeletionService _rankRoleDeletionService;
    private readonly RankRoleViewingService _rankRoleViewingService;

    public RankRoleManagementChannel(ILogger<RankRoleManagementChannel> logger,
        ErrorHandlingService errorHandlingService,
        RankRoleCreationService rankRoleCreationService,
        RankRoleDeletionService rankRoleDeletionService,
        RankRoleViewingService rankRoleViewingService) : base(logger, errorHandlingService, "Rank Role")
    {
        _rankRoleCreationService = rankRoleCreationService;
        _rankRoleDeletionService = rankRoleDeletionService;
        _rankRoleViewingService = rankRoleViewingService;
    }

    protected override async ValueTask HandleMessage(RankRoleManagementAction message)
    {
        switch (message.Action)
        {
            case RankRoleManagementActionType.Create:
                await _rankRoleCreationService.Create(message);
                break;
            case RankRoleManagementActionType.Delete:
                await _rankRoleDeletionService.Delete(message);
                break;
            case RankRoleManagementActionType.View:
                _rankRoleViewingService.View(message);
                break;
        }
    }
}