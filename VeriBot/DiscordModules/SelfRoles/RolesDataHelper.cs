using DSharpPlus.Entities;
using VeriBot.DiscordModules.SelfRoles.Services;
using VeriBot.Responders;

namespace VeriBot.DiscordModules.SelfRoles;

public class RolesDataHelper
{
    private readonly SelfRoleViewingService _selfRoleViewingService;

    public RolesDataHelper(SelfRoleViewingService selfRoleViewingService)
    {
        _selfRoleViewingService = selfRoleViewingService;
    }

    // TODO: Move to channel.
    public void DisplayRoles(DiscordGuild guild, IResponder responder) => _selfRoleViewingService.DisplaySelfRoles(guild, responder);
}