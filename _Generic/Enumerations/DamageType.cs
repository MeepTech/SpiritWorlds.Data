using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;

namespace SpiritWorlds.Data {

  /// <summary>
  /// Used to indicate different types of damage.
  /// </summary>
  public class DamageType : Enumeration<DamageType> {

    /// <summary>
    /// General damage of any kind
    /// </summary>
    public static DamageType General {
      get;
    } = new DamageType(
      "",
      "General",
      "Damage of any kind",
      '*'
    );

    /// <summary>
    /// The name of this damage type
    /// </summary>
    public string Name { 
      get;
    }

    /// <summary>
    /// A description of this damage type
    /// </summary>
    public string Description {
      get; 
    }

    /// <summary>
    /// The letter/char used to represent this damage type
    /// </summary>
    public char LetterRepresentation {
      get;
    }
    
    /// <summary>
    /// The default damage multiplier stat deriver
    /// </summary>
    public DerivedStat? DefaultDamageMultiplierStat { 
      get;
    }

    /// <summary>
    /// The default damage reistance stat deriver
    /// </summary>
    public DerivedStat? DefaultDamageResistanceStat { 
      get; 
    }

    protected DamageType(string keyPrefix, string uniqueName, string description, char letterRepresentation, DerivedStat? defaultDamageMultiplierStat = null, DerivedStat? defaultDamageResistanceStat = null)
      : base(keyPrefix + uniqueName) {
      Name = uniqueName;
      Description = description;
      LetterRepresentation = letterRepresentation;
      DefaultDamageMultiplierStat = defaultDamageMultiplierStat;
      DefaultDamageResistanceStat = defaultDamageResistanceStat;
    }
  }
}