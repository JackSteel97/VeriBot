using DSharpPlus.Entities;
using VeriBot.Channels;
using VeriBot.Responders;

namespace VeriBot.Channels.Pets;

public enum PetCommandActionType
{
    Search,
    ManageAll,
    Treat,
    ManageOne,
    ViewBonuses,
    View,
    CheckForDeath
}

public record PetCommandAction : BaseAction<PetCommandActionType>
{
    public DiscordMember Target { get; }
    public long PetId { get; init; }

    public PetCommandAction(PetCommandActionType action, IResponder responder, DiscordMember member, DiscordGuild guild, DiscordMember target = null)
        : base(action, responder, member, guild)
    {
        Target = target ?? member;
    }

    public PetCommandAction(PetCommandActionType action, IResponder responder, DiscordMember member, DiscordGuild guild, long petId, DiscordMember target = null)
        : this(action, responder, member, guild, target)
    {
        PetId = petId;
    }
}