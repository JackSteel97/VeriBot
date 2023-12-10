﻿using Humanizer;
using Humanizer.Localisation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using VeriBot.DiscordModules.Pets.Enums;

namespace VeriBot.Database.Models.Pets;

[DebuggerDisplay("{Rarity} {Species} {Name}")]
public class Pet : IEquatable<Pet>
{
    public long RowId { get; set; }
    public ulong OwnerDiscordId { get; set; }

    [MaxLength(70)] public string Name { get; set; }

    public int Priority { get; set; }
    public double EarnedXp { get; set; }
    public int CurrentLevel { get; set; } = 1;
    public DateTime BornAt { get; set; }
    public DateTime FoundAt { get; set; }
    public Species Species { get; set; }
    public Size Size { get; set; }
    public Rarity Rarity { get; set; }
    public bool IsCorrupt { get; set; }
    public List<PetAttribute> Attributes { get; set; }
    public List<PetBonus> Bonuses { get; set; }
    public bool IsDead { get; set; }

    public bool IsPrimary => Priority == 0;

    public override string ToString()
    {
        return $"A {Rarity} {Age.Humanize(maxUnit: TimeUnit.Year)} old, {Size} {Species.GetName()} with\n\t{string.Join("\n\t", Attributes.Select(a => $"{a.Description} {a.Name}"))}";
    }

    public string ShortDescription => $"{Size} {Rarity} {Species} - {Name} ({Age.Humanize(maxUnit: TimeUnit.Year)} old)";

    public TimeSpan Age => DateTime.UtcNow - BornAt;

    public string GetName() => Name?.Trim() ?? $"Unnamed {Species.GetName()}";

    public void AddBonuses(List<PetBonus> bonuses)
    {
        foreach (var bonus in bonuses) AddBonus(bonus);
    }

    public void AddBonus(PetBonus bonus)
    {
        if (Bonuses == default) Bonuses = new List<PetBonus>(1);

        var existingBonusOfThisType = Bonuses.Find(b => b.BonusType == bonus.BonusType);
        if (existingBonusOfThisType != null)
            existingBonusOfThisType.Value += bonus.Value;
        else
            Bonuses.Add(bonus);
    }

    /// <summary>
    ///     Deep clone.
    /// </summary>
    /// <returns>A deep clone of this object.</returns>
    public Pet Clone()
    {
        var clone = (Pet)MemberwiseClone();
        clone.Attributes = Attributes.ConvertAll(x => x.Clone());
        clone.Bonuses = Bonuses.ConvertAll(x => x.Clone());

        return clone;
    }

    /// <inheritdoc />
    public bool Equals(Pet other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return RowId == other.RowId && OwnerDiscordId == other.OwnerDiscordId;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Pet)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(RowId, OwnerDiscordId);

    public static bool operator ==(Pet left, Pet right) => Equals(left, right);

    public static bool operator !=(Pet left, Pet right) => !Equals(left, right);
}