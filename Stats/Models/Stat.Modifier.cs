using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SpiritWorlds.Data {

  public partial struct Stat {

    /// <summary>
    /// Make a new type of modifier as an enumeration item so it can be found via a key.
    /// </summary>
    [Serializable]
    public partial struct Modifier {

      /// <summary>
      /// Which value of the stat this modifier modifies.
      /// This is mostly for DepleteableStats
      /// </summary>
      [JsonConverter(typeof(StringEnumConverter))] 
      public enum Targets {
        CurrentValue,
        UpperBar
      }

      /// <summary>
      /// The name of this modifier. Used as a key on the applied stat.
      /// </summary>
      public string Name { get; }

      /// <summary>
      /// A short description of this modifier, and why it's specifically applied.
      /// </summary>
      public string Description { get; }

      /// <summary>
      /// What this modifier modifies about the stat.
      /// </summary>
      public Targets Target { get; }

      /// <summary>
      /// The types of modifier logic to apply for this modification
      /// </summary>
      public IEnumerable<Applier> Appliers {
        get;
      }

      /// <summary>
      /// Make a stat modifier
      /// </summary>
      public Modifier(string name, IEnumerable<Applier> appliers, string description = null, Targets target = Targets.CurrentValue) {
        Name = name;
        Target = target;
        Description = description;
        Appliers = appliers;
      }

      public override int GetHashCode()
        => Name.GetHashCode();
    }
  }
}
