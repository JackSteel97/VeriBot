﻿using System;
using System.Diagnostics;
using VeriBot.DiscordModules.Pets.Enums;

namespace VeriBot.Database.Models.Pets;

[DebuggerDisplay("{BonusType} : {Value}")]
public class PetBonus
{
    public long RowId { get; set; }
    public long PetId { get; set; }
    public Pet Pet { get; set; }
    public BonusType BonusType { get; set; }
    public double Value { get; set; }

    /// <summary>
    ///     Shallow clones the bonus, <see cref="Pet" /> is nullified during this to prevent accidental alterations.
    /// </summary>
    /// <returns>A clone of the bonus</returns>
    public PetBonus Clone()
    {
        var clone = (PetBonus)MemberwiseClone();
        clone.Pet = null;
        return clone;
    }

    public bool HasNegativeEffect()
    {
        bool isPenaltyType = BonusType.IsPenalty();
        return isPenaltyType && Value > 0 || !isPenaltyType && Value <= 0;
    }

    public void LevelUp()
    {
        if (BonusType == BonusType.PetSlots) return;
        double increase = Math.Abs(Value * 0.02);
        if (BonusType.IsPenalty() && HasNegativeEffect())
            // Penalty bonuses should get better by levelling.
            increase *= -1;
        Value += increase;
    }
}