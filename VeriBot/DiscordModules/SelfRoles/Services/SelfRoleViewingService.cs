using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeriBot.Database.Models;
using VeriBot.DataProviders.SubProviders;
using VeriBot.DiscordModules.Config;
using VeriBot.DiscordModules.SelfRoles.Helpers;
using VeriBot.Helpers;
using VeriBot.Responders;
using VeriBot.Services;

namespace VeriBot.DiscordModules.SelfRoles.Services;

public class SelfRoleViewingService
{
    private readonly ConfigDataHelper _configDataHelper;
    private readonly ErrorHandlingService _errorHandlingService;
    private readonly SelfRolesProvider _selfRolesProvider;
    private readonly GuildsProvider _guildsProvider;

    public SelfRoleViewingService(ConfigDataHelper configDataHelper,
        SelfRolesProvider selfRolesProvider,
        ErrorHandlingService errorHandlingService,
        GuildsProvider guildsProvider)
    {
        _configDataHelper = configDataHelper;
        _selfRolesProvider = selfRolesProvider;
        _errorHandlingService = errorHandlingService;
        _guildsProvider = guildsProvider;
    }

    public async Task SendReactionMessage(DiscordClient client, DiscordGuild guild, DiscordChannel channel)
    {
        if (!_selfRolesProvider.TryGetGuildRoles(guild.Id, out var allRoles))
        {
            return;
        }

        var embedBuilder = new DiscordEmbedBuilder().WithColor(EmbedGenerator.InfoColour).WithTitle("Give yourself a role");
        var rolesBuilder = new StringBuilder();
        AppendSelfRoles(client, guild, allRoles, rolesBuilder);
        embedBuilder.WithDescription($"Use `/SelfRoles Join \"RoleName\"` to join one of these roles, or react to this message with the corresponding emoji.\n\n{rolesBuilder}");

        var messageBuilder = new DiscordMessageBuilder().WithEmbed(embedBuilder.Build());
        var message = await channel.SendMessageAsync(messageBuilder);
        var existingMessage = _guildsProvider.GetGuildSelfRoleAssignmentMessageId(guild.Id);
        if (existingMessage.channel.HasValue && existingMessage.message.HasValue)
        {
            DiscordMessage messageToDelete = null;
            try
            {
                messageToDelete = await guild.GetChannel(existingMessage.channel.Value).GetMessageAsync(existingMessage.message.Value);
            }
            catch (Exception e)
            {
                // Could not find message to delete - probably this first use of this.
            }

            if (messageToDelete != null)
            {
                await messageToDelete.DeleteAsync();
            }
        }

        await _guildsProvider.UpdateGuildSelfRoleAssignmentMessageId(guild.Id, channel.Id, message.Id);
        foreach (var role in allRoles)
        {
            if (!role.EmojiId.HasValue) continue;
            var emoji = DiscordEmoji.FromGuildEmote(client, role.EmojiId.Value);
            await message.CreateReactionAsync(emoji);
        }
    }

    public void DisplaySelfRoles(DiscordGuild guild, IResponder responder, DiscordClient client)
    {
        if (!_selfRolesProvider.TryGetGuildRoles(guild.Id, out var allRoles))
        {
            responder.Respond(SelfRoleMessages.NoSelfRolesAvailable());
            return;
        }

        string prefix = _configDataHelper.GetPrefix(guild.Id);

        var builder = new DiscordEmbedBuilder()
            .WithColor(EmbedGenerator.InfoColour)
            .WithTitle("Available Self Roles");

        var rolesBuilder = new StringBuilder();
        rolesBuilder.Append(Formatter.Bold("All")).AppendLine(" - Join all available Self Roles");

        AppendSelfRoles(client, guild, allRoles, rolesBuilder);

        builder.WithDescription($"Use `{prefix}SelfRoles Join \"RoleName\"` to join one of these roles.\n\n{rolesBuilder}");
        responder.Respond(new DiscordMessageBuilder().AddEmbed(builder.Build()));
    }

    private static void AppendSelfRoles(DiscordClient client, DiscordGuild guild, List<SelfRole> allRoles, StringBuilder rolesBuilder)
    {
        foreach (var role in allRoles)
        {
            var discordRole = guild.Roles.Values.FirstOrDefault(r => r.Name.Equals(role.RoleName, StringComparison.OrdinalIgnoreCase));
            string roleMention = role.RoleName;
            if (discordRole != default) roleMention = discordRole.Mention;
            
            if (role.EmojiId.HasValue)
            {
                var emoji = DiscordEmoji.FromGuildEmote(client, role.EmojiId.Value);
                rolesBuilder.Append(emoji.ToString()).Append(" - ");
            }
            rolesBuilder.Append(Formatter.Bold(roleMention));
            rolesBuilder.Append(" - ").AppendLine(role.Description).AppendLine();
           
        }
    }
}