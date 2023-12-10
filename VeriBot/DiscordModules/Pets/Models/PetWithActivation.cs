using VeriBot.Database.Models.Pets;

namespace VeriBot.DiscordModules.Pets.Models;

public readonly record struct PetWithActivation(Pet Pet, bool Active);