using System.Collections.Generic;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A stat
  /// </summary>
  public interface IStat {

    /// <summary>
    /// The archetype of this stat
    /// </summary>
    Stat.Type Archetype { get; }

    /// <summary>
    /// The base stat, without modiifiers applied.
    /// </summary>
    float BaseValue { get; }

    /// <summary>
    /// The current value with all the modifiers applied.
    /// </summary>
    float CurrentValue { get; }

    /// <summary>
    /// Modifiers currently applied to this stat.
    /// </summary>
    IEnumerable<Stat.Modifier> Modifiers { get; }

    /// <summary>
    /// Copy this stat and add a modifier
    /// </summary>
    IStat WithModifier(Stat.Modifier toAdd);

    /// <summary>
    /// Copy this stat and add or override a modifier
    /// </summary>
    IStat WithModifierOverriden(Stat.Modifier toAddOrOverride);

    /// <summary>
    /// Copy this stat and add a modifier
    /// </summary>
    IStat WithModifiers(IEnumerable<Stat.Modifier> toAdd);

    /// <summary>
    /// Copy this stat and add or override a modifier
    /// </summary>
    IStat WithModifiersOverriden(IEnumerable<Stat.Modifier> toAddOrOverride);

    /// <summary>
    /// Copy this stat and remove a modifier
    /// </summary>
    IStat WithoutModifier(string toRemove);

    /// <summary>
    /// Copy this stat and remove a modifier
    /// </summary>
    IStat WithoutModifiers(IEnumerable<string> toRemove);

    /// <summary>
    /// Copy this stat with a new base value
    /// </summary>
    IStat WithUpdatedBaseValue(float? newBase = null);
  }
}