﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace VeriBot.Database.Models.Pets;

[DebuggerDisplay("{Name} - {Description}")]
public class PetAttribute
{
    public long RowId { get; set; }
    public long PetId { get; set; }
    public Pet Pet { get; set; }

    [MaxLength(255)] public string Name { get; set; }

    [MaxLength(255)] public string Description { get; set; }

    public PetAttribute Clone()
    {
        var clone = (PetAttribute)MemberwiseClone();
        Pet = null;
        return clone;
    }
}