using DSharpPlus.Entities;
using VeriBot.Responders;

namespace VeriBot.Channels;

public record BaseAction<TAction>(TAction Action, IResponder Responder, DiscordMember Member, DiscordGuild Guild);