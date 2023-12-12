using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VeriBot.Helpers;
using VeriBot.Services.Configuration;

namespace VeriBot.Services;

public class NotifierService 
{
    private readonly DiscordClient _client;
    private readonly ILogger<NotifierService> _logger;

    private readonly ulong _commonServerId;
    private readonly ulong _haslamUserId;
    public NotifierService(AppConfigurationService appConfigurationService, DiscordClient client, ILogger<NotifierService> logger)
    {
        _client = client;
        _logger = logger;

        _commonServerId = appConfigurationService.Application.CommonServerId;
        _haslamUserId = appConfigurationService.Application.HaslamUserId;
    }


    public async Task SendRoleGrantedMessageToHaslam(DiscordMember member, string triggerRoleName, string grantedRoleName)
    {
        _logger.LogInformation("Attempting to send role granted message to Haslam for {UserId} with trigger role {TriggerRole}, and granted role {GrantedRole}", member.Id, triggerRoleName, grantedRoleName);
        var commonServer = await _client.GetGuildAsync(_commonServerId);
        var haslam = await commonServer.GetMemberAsync(_haslamUserId);

        var message = EmbedGenerator.Info($"{Formatter.Bold(member.DisplayName)} was assigned the {Formatter.Bold(triggerRoleName)} role therefore I have granted them the {Formatter.Bold(grantedRoleName)} role", "Role Granted");
        await haslam.SendMessageAsync(message);
        _logger.LogInformation("Successfully sent role granted message to Haslam for {UserId} with trigger role {TriggerRole}, and granted role {GrantedRole}", member.Id, triggerRoleName, grantedRoleName);
    }
    
    public async Task SendRoleRevokedMessageToHaslam(DiscordMember member, string revokedRoleName)
    {
        _logger.LogInformation("Attempting to send role revoked message to Haslam for {UserId} and revoked role {RevokedRole}", member.Id, revokedRoleName);
        var commonServer = await _client.GetGuildAsync(_commonServerId);
        var haslam = await commonServer.GetMemberAsync(_haslamUserId);

        var message = EmbedGenerator.Info($"{Formatter.Bold(member.DisplayName)} no longer has any of the required roles therefore I have revoked the {Formatter.Bold(revokedRoleName)} role from them", "Role Revoked");
        await haslam.SendMessageAsync(message);
        _logger.LogInformation("Successfully sent role revoked message to Haslam for {UserId} and revoked role {RevokedRole}", member.Id, revokedRoleName);
    }
}