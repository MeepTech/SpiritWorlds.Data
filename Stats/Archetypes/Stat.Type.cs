using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {
  public partial struct Stat {
    /// <summary>
    /// The Base Archetype for Stats
    /// </summary>
    public abstract class Type : Archetype<Stat, Stat.Type> {

      /// <summary>
      /// The acronym used for this stat.
      /// 3-2 letters.
      /// </summary>
      public string Acronym {
        get; 
      }

      /// <summary>
      /// A general description of this type
      /// </summary>
      public string Description {
        get;
      }

      /// <summary>
      /// The default Max Cap of this stat's BaseValue
      /// Modifications can push the stat's CurrentValue above this.
      /// </summary>
      public virtual float DefaultUpperBar {
        get;
      } = 50;

      /// <summary>
      /// The default Min Cap of this stat's BaseValue
      /// Modifications can push the stat's CurrentValue below this.
      /// </summary>
      public virtual float DefaultLowerBar {
        get;
      } = 0;

      /// <summary>
      /// The default Min Cap of this stat's BaseValue
      /// Modifications can push the stat's CurrentValue below this.
      /// </summary>
      public virtual float DefaultInitialValue {
        get;
      } = 1;

      /// <summary>
      /// Used to make new Child Archetypes for Stat.Type 
      /// </summary>
      /// <param name="keyPrefix">Can be used to namespace this type.</param>
      protected Type(string keyPrefix, string name, string acronym, string description, int? upperBar = default, int? lowerBar = default, int? defaultValue = default)
        : base(new Identity(name, keyPrefix), collection: Stat.Types) 
      { 
        Acronym = acronym;
        Description = description;
        if (upperBar.HasValue) DefaultUpperBar = upperBar.Value; 
        if (lowerBar.HasValue) DefaultLowerBar = lowerBar.Value; 
        if (defaultValue.HasValue) DefaultInitialValue = defaultValue.Value;
      }

      /// <summary>
      /// Used to make a new stat of the given type.
      /// </summary>
      public Stat Make(float? baseValue = null, IEnumerable<Modifier> modifiers = null)
        => Make(
          (nameof(Stat.BaseValue), baseValue),
          (nameof(Stat.Modifiers), modifiers)
        );

      /// <summary>
      /// Used to make a new stat of the given type.
      /// </summary>
      public DepleteableStat MakeDepleteable(float? baseValue = null, IEnumerable<Modifier> modifiers = null)
        => new(
          this,
          baseValue,
          modifiers?.ToDictionary(m => m.Name)
        );
    }
  }
}
