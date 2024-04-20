using DSharpPlus;
using DSharpPlus.Entities;
using System.Threading.Tasks;
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
    
    public void DisplayRoles(DiscordClient client, DiscordGuild guild, IResponder responder) => _selfRoleViewingService.DisplaySelfRoles(guild, responder, client);
    public Task SendReactionMessage(DiscordClient client, DiscordGuild guild, DiscordChannel channel) => _selfRoleViewingService.SendReactionMessage(client, guild, channel);
}