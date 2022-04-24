using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A stat who's temporary value can be changed between the min allowed by the archetype and the CurrentValue.
  /// The current value and base value properties represent the "Upper Bar" of the stat, and the actual current value is represent by the temp value.
  /// </summary>
  public partial struct DepleteableStat
    : Model<Stat, Stat.Type>.IFromInterface, IModel.IUseDefaultUniverse, IStat {

    ///<summary><inheritdoc/></summary>
    public float BaseValue
      => _base; float _base;

    ///<summary><inheritdoc/></summary>
    public float CurrentValue {
      get {
        float value = _base;
        DepleteableStat @this = this;
        foreach (Stat.Modifier modifier in Modifiers.Where(m => m.Target == Stat.Modifier.Targets.UpperBar)) {
          value += modifier.Appliers
            .Sum(a => a.GetBonus(@this, value));
        }

        return value;
      }
    }

    /// <summary>
    /// The temporary value of this stat. This is for depletable stats.
    /// This has modifiers applied.
    /// Returns current value if it would overflow it.
    /// </summary>
    public float TemporaryCurrentValue {
      get {
        float value = _base;
        DepleteableStat @this = this;
        foreach (Stat.Modifier modifier in Modifiers.Where(m => m.Target == Stat.Modifier.Targets.CurrentValue)) {
          value += modifier.Appliers
            .Sum(a => a.GetBonus(@this, value));
        }

        return Math.Min(value, CurrentValue);
      }
    }

    /// <summary>
    /// The temporary value of this stat. This is for depletable stats.
    /// This does not have modifiers applied
    /// Returns base value if it would overflow it.
    /// </summary>
    public float TemporaryBaseValue
      => Math.Min(_tempBase, BaseValue); float _tempBase;

    ///<summary><inheritdoc/></summary>
    public Stat.Type Archetype {
      get;
      private set;
    } Stat.Type Model<Stat, Stat.Type>.IFromInterface.Archetype {
      get => Archetype;
      set => Archetype = value;
    }

    ///<summary><inheritdoc/></summary>
    public IEnumerable<Stat.Modifier> Modifiers
      => _modifiers?.Values ?? Enumerable.Empty<Stat.Modifier>(); internal Dictionary<string, Stat.Modifier> _modifiers;

    DepleteableStat(IBuilder<Stat> builder) : this(builder.Archetype as Stat.Type,
      builder.GetParam(nameof(BaseValue), (builder.Archetype as Stat.Type).DefaultInitialValue),
      builder.GetParam(nameof(Modifiers), Enumerable.Empty<Stat.Modifier>()).ToDictionary(m => m.Name)
    ) {} internal DepleteableStat(Stat.Type type, float? @base, Dictionary<string, Stat.Modifier> modifiers) : this() {
      Archetype ??= type;
      _modifiers = modifiers;
      _base = _tempBase = @base ?? Archetype.DefaultInitialValue;
    }

    /// <summary>
    /// Copy this stat and update the temp base value.
    /// </summary>
    public DepleteableStat WithUpdatedTempBaseValue(float? newTempBase = null) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value);
      copy._tempBase = newTempBase ?? Archetype.DefaultInitialValue;
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithUpdatedBaseValue(float? newBase = null) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value);
      copy._base = newBase ?? Archetype.DefaultInitialValue;
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifiersOverriden(IEnumerable<Stat.Modifier> toAddOrOverride) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      toAddOrOverride.ForEach(m => copy._modifiers[m.Name] = m);
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifierOverriden(Stat.Modifier toAddOrOverride) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      copy._modifiers[toAddOrOverride.Name] = toAddOrOverride;
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifiers(IEnumerable<Stat.Modifier> toAdd) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      toAdd.ForEach(m => copy._modifiers.Add(m.Name, m));
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithModifier(Stat.Modifier toAdd) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value) ?? new();
      copy._modifiers.Add(toAdd.Name, toAdd);
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithoutModifiers(IEnumerable<string> toRemove) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value);
      toRemove.ForEach(m => copy._modifiers?.Remove(m));
      return copy;
    }

    ///<summary><inheritdoc/></summary>
    public IStat WithoutModifier(string toRemove) {
      DepleteableStat copy = this;
      copy._modifiers = _modifiers?.ToDictionary(m => m.Key, m => m.Value);
      copy._modifiers?.Remove(toRemove);
      return copy;
    }
  }
}
