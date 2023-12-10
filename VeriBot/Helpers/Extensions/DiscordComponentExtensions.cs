using DSharpPlus.Entities;

namespace VeriBot.Helpers.Extensions;

public static class DiscordComponentExtensions
{
    public static DiscordButtonComponent Disable(this DiscordButtonComponent component, bool disable = true) => disable ? new DiscordButtonComponent(component).Disable() : component;
}