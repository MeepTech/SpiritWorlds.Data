using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  /// <summary>
  /// The Base Model for all Stats
  /// </summary>
  public partial struct Stat
    : Model<Stat, Stat.Type>.IFromInterface, IModel.IUseDefaultUniverse, IStat {

    ///<summary><inheritdoc/></summary>
    public static Stat.Type.Collection Types {
      get;
    } = new();

    ///<summary><inheritdoc/></summary>
    public float BaseValue
      => _base; float _base;

    ///<summary><inheritdoc/></summary>
    public float CurrentValue {
      get {
        float value = _base;
        Stat @this = this;
        foreach (Modifier modifier in Modifiers.Where(m => m.Target == Modifier.Targets.CurrentValue)) {
          value += modifier.Appliers
            .Sum(a => a.GetBonus(@this, value));
        }

        return value;
      }
    }

    /// <summary>
    /// The upper bar of this stat.
    /// </summary>
    /*public float UpperBar
      => Archetype.DefaultUpperBar;*/

    ///<summary><inheritdoc/></summary>
    public Type Archetype {
      get;
      private set;
    } Type Model<Stat, Type>.IFromInterface.Archetype {
      get => Archetype;
      set => Archetype = value;
    }

    ///<summary><inheritdoc/></summary>
    public IEnumerable<Modifier> Modifiers
      => _modifiers?.Values ?? Enumerable.Empty<Modifier>(); Dictionary<string, Modifier> _modifiers;

    Stat(IBuilder<Stat> builder) : this() {
      _modifiers = builder.GetParam(nameof(Modifiers), Enumerable.Empty<Modifier>()).ToDictionary(m => m.Name);
      _base = builder.GetParam<float?>(nameof(BaseValue), (builder.Archetype as Stat.Type).DefaultInitialValue).Value;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithUpdatedBaseValue(float? newBase = null) {
      Stat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value);
      copy._base = newBase ?? Archetype.DefaultInitialValue;
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifiersOverriden(IEnumerable<Stat.Modifier> toAddOrOverride) {
      Stat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      toAddOrOverride.ForEach(m => copy._modifiers[m.Name] = m);
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifierOverriden(Stat.Modifier toAddOrOverride) {
      Stat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      copy._modifiers[toAddOrOverride.Name] = toAddOrOverride;
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifiers(IEnumerable<Stat.Modifier> toAdd) {
      Stat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      toAdd.ForEach(m => copy._modifiers.Add(m.Name, m));
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifier(Stat.Modifier toAdd) {
      Stat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      copy._modifiers.Add(toAdd.Name, toAdd);
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithoutModifiers(IEnumerable<string> toRemove) {
      Stat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value);
      toRemove.ForEach(m => copy._modifiers?.Remove(m));
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithoutModifier(string toRemove) {
      Stat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value);
      copy._modifiers?.Remove(toRemove);
      return copy;
    }
  }
}
