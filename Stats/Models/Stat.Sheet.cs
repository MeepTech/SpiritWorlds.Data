using Meep.Tech.Data;
using SpiritWorlds.Data.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  public partial struct Stat {

    /// <summary>
    /// A sheet of stats
    /// </summary>
    public partial class Sheet : Model<Stat.Sheet, Stat.Sheet.Type>, IModel, IEnumerable<IStat> {
      Dictionary<Stat.Type, IStat> _baseStats;
      IReadOnlyDictionary<Stat.Type, DerivedStat> _derivedStats
        = new Dictionary<Stat.Type, DerivedStat>();

      /// <summary>
      /// The number of different stats in this sheet.
      /// </summary>
      public int Count 
        => _baseStats.Count + _derivedStats.Count;

      /// <summary>
      /// The types of stats in this sheet
      /// </summary>
      public IEnumerable<Stat.Type> StatTypes 
        => _baseStats.Keys.Concat(_derivedStats.Keys);

      /// <summary>
      /// All stats in this sheet. Same as enumerator
      /// </summary>
      public IEnumerable<IStat> Stats 
        => _baseStats.Values.Concat(_derivedStats.Select(s => Get(s.Key)));

      /// <summary>
      /// The base stats in this sheet. These can be modified directly
      /// </summary>
      public IEnumerable<Stat.Type> BaseStats 
        => _baseStats.Keys;

      /// <summary>
      /// The derived stats in this sheet. These are based on other stats and can't be changed directly.
      /// </summary>
      public IEnumerable<Stat.Type> DerivedStats 
        => _derivedStats.Keys;

      /// <summary>
      /// The derived stats in this sheet. These are based on other stats and can't be changed directly.
      /// </summary>
      public IReadOnlyDictionary<Stat.Type, IEnumerable<Modifier>> DerivedStatModifiers
        => _derivedStats.ToDictionary(e => e.Key, e => e.Value.Modifiers.Values.AsEnumerable());

      /// <summary>
      /// links to stat sheets that this one is dependent on.
      /// </summary>
      public Dictionary<Stat.Sheet.Type, Sheet> Dependencies {
        get;
      } = new();

      /// <summary>
      /// The stat provider for this sheet, if there is one.
      /// </summary>
      public StatsProvider Provider {
        get;
        internal set;
      }

      /// <summary>
      /// Get a stat by type.
      /// </summary>
      public IStat this[Stat.Type key] {
        get => Get(key);
        protected set => _baseStats[key] = value;
      }

      /// <summary>
      /// Make a new simple stat sheet from some stats.
      /// </summary>
      public Sheet(
        HashSet<IStat> stats,
        Dictionary<Stat.Sheet.Type, Stat.Sheet> dependencies = null
      ) : this() {
        _baseStats = stats.ToDictionary(s => s.Archetype);
        Dependencies = dependencies ?? Dependencies;
      }

      /// <summary>
      /// Used for internal Make function.
      /// </summary>
      internal Sheet(
        HashSet<IStat> baseStats,
        IReadOnlyDictionary<Stat.Type, DerivedStat> derivedStats,
        Dictionary<Stat.Sheet.Type, Stat.Sheet> dependencies = null
      ) : this() {
        _baseStats = baseStats.ToDictionary(s => s.Archetype);
        _derivedStats = derivedStats ?? _derivedStats;
        Dependencies = dependencies ?? Dependencies;
      } Sheet() { }

      /// <summary>
      /// Get a stat from the sheet
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      public IStat Get(Stat.Type type) {
        if (_baseStats.TryGetValue(type, out var found)) {
          return found;
        } else return _derivedStats[type].Calculator(this);
      }

      /// <summary>
      /// Try to get a stat from this sheet if it exists.
      /// </summary>
      public bool TryToGet(Stat.Type type, out IStat value) {
        if (_baseStats.TryGetValue(type, out var found)) {
          value = found;
          return true;
        } else if (_derivedStats.TryGetValue(type, out var foundDerriver)) {
          value = foundDerriver.Calculator(this);
          if (value is Stat stat) {
            stat._modifiers = foundDerriver.Modifiers;
            value = stat;
          } else if (value is DepleteableStat dStat) {
            dStat._modifiers = foundDerriver.Modifiers;
            value = dStat;
          }
          return true;
        } else {
          value = default;
          return false;
        }
      }

      /// <summary>
      /// Check if this sheet contains a given stat
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public bool Contains(Stat.Type key) {
        return _baseStats.ContainsKey(key) || _derivedStats.ContainsKey(key);
      }

      /// <summary>
      /// Attempts to modify the given base stat's base value, throwing if no base stat was found or nothing was ableto me modified.
      /// </summary>
      public void ModifyBase(Stat.Type type, float newBaseValue, out IStat previousValue) {
        previousValue = _baseStats[type];
        _baseStats[type] = _baseStats[type].WithUpdatedBaseValue(newBaseValue);
      }

      /// <summary>
      /// Attempts to modify the given base stat's base value.
      /// </summary>
      /// <returns>False if nothing was able to be modified or the stat wasn't found in the sheet</returns>
      public bool TryToModifyBase(Stat.Type type, float newBaseValue, out IStat? previousValue) {
        if (_baseStats.TryGetValue(type, out var previous)) {
          previousValue = previous;
          _baseStats[type] = _baseStats[type].WithUpdatedBaseValue(newBaseValue);
          return true;
        }

        previousValue = null;
        return false;
      }

      /// <summary>
      /// Attempts to modify the given base stat, throwing if no base stat was found or the modifier already exists or couldn't be added.
      /// </summary>
      public void AddModifier(Stat.Type type, Modifier toAdd, out IStat previousValue) {
        if (_derivedStats.ContainsKey(type)) {
          previousValue = Get(type);
          _derivedStats[type].Modifiers.Add(toAdd.Name, toAdd);
        } else {
          previousValue = _baseStats[type];
          _baseStats[type] = _baseStats[type].WithModifier(toAdd);
        }
      }

      /// <summary>
      /// Attempts to modify the given base stat, throwing if no base stat was found or nothing was ableto me modified.
      /// </summary>
      public bool TryToAddOrUpdateModifier(Stat.Type type, Modifier toAddOrUpdate, out IStat? previousValue) {
        if (_baseStats.TryGetValue(type, out var previous)) {
          previousValue = previous;
          _baseStats[type] = _baseStats[type].WithModifierOverriden(toAddOrUpdate);
          return true;
        } else if (_derivedStats.ContainsKey(type)) {
          previousValue = Get(type);
          _derivedStats[type].Modifiers[toAddOrUpdate.Name] = toAddOrUpdate;
          return true;
        } else {
          previousValue = null;
          return false;
        }
      }

      /// <summary>
      /// Attempts to modify the given base stat. Throws if the requested stat is not found.
      /// </summary>
      /// <returns>False if nothing was able to be removed</returns>
      public bool RemoveModifier(Stat.Type type, string toRemove, out IStat previousValue) {
        if (_derivedStats.ContainsKey(type)) {
          previousValue = Get(type);
          return _derivedStats[type].Modifiers.Remove(toRemove);
        } else {
          previousValue = _baseStats[type];
          bool hadModifier = previousValue.Modifiers.Any(m => m.Name == toRemove);
          _baseStats[type] = _baseStats[type].WithoutModifier(toRemove);
          return hadModifier;
        }
      }

      /// <summary>
      /// Attempts to modify the given base stat.
      /// </summary>
      /// <returns>False if nothing was able to be removed</returns>
      public bool TryToRemoveModifier(Stat.Type type, string toRemove, out IStat previousValue) {
        if (_baseStats.TryGetValue(type, out var previous)) {
          previousValue = _baseStats[type];
          bool hadModifier = previousValue.Modifiers.Any(m => m.Name == toRemove);
          _baseStats[type] = _baseStats[type].WithoutModifier(toRemove);
          return hadModifier;
        } else if (_derivedStats.ContainsKey(type)) {
          previousValue = Get(type);
          return _derivedStats[type].Modifiers.Remove(toRemove);
        } else {
          previousValue = null;
          return false;
        }
      }

      #region IEnumerable

      public IEnumerator<IStat> GetEnumerator()
        => Stats.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

      #endregion
    }
  }
}
