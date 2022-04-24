using SpiritWorlds.Data.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A stat in a sheet that can be derived from other stats or values.
  /// </summary>
  public struct DerivedStat {

    /// <summary>
    /// Used to make a new stat from existing stats.
    /// </summary>
    public delegate IStat DeriveNewStatFromExistingStatsCalculator(Stat.Sheet sheet, params Stat[] stats);

    public IEnumerable<Stat.Type> InfluencedBy { get; }
    public Func<Stat.Sheet, IStat> Calculator{ get; }
    public Dictionary<string, Stat.Modifier> Modifiers{ get; }

    public DerivedStat(IEnumerable<Stat.Type> influencedBy, Func<Stat.Sheet, IStat> calculator, IEnumerable<Stat.Modifier> modifiers = null) {
      InfluencedBy = influencedBy;
      Calculator = calculator;
      Modifiers = modifiers?.ToDictionary(m => m.Name) ?? new();
    }

    /// <summary>
    /// Make a derived stat based on other stats scaled to a given value.
    /// </summary>
    /*public static DerivedStat BasedOnScaledValues(
      Stat.Type statToBuild,
      Func<Stat.Sheet, IEnumerable<(Stat.Sheet sheet, Stat.Type type, float scale)>> inputStatScalesAddingUpTo1,
      Func<float, float> andThen = null
    ) => new(
      inputStatScalesAddingUpTo1(null).Select(v => v.type),
      parentSheet => {
        float value = 0;
        foreach ((Stat.Sheet sheet, Stat.Type type, float scale) in inputStatScalesAddingUpTo1(parentSheet)) {
          value += sheet.Get(type).CurrentValue * scale;
        }
        value = andThen?.Invoke(value) ?? value;
        return statToBuild.Make(value);
      }
    );

    /// <summary>
    /// Make a derived stat based on other stats scaled to a given value.
    /// </summary>
    public static DerivedStat BasedOnScaledValues<TStat>(
      Func<Stat.Sheet, IEnumerable<(Stat.Sheet sheet, Stat.Type type, float scale)>> inputStatScalesAddingUpTo1,
      Func<float, float> andThen = null
    ) where TStat : Stat.Type
      => new(
      inputStatScalesAddingUpTo1(null).Select(v => v.type),
      parentSheet => {
        float value = 0;
        foreach ((Stat.Sheet sheet, Stat.Type type, float scale) in inputStatScalesAddingUpTo1(parentSheet)) {
          value += sheet.Get(type).CurrentValue * scale;
        }
        value = andThen?.Invoke(value) ?? value;
        return Stat.Types.Get<TStat>().Make(value);
      }
    );*/

    /// <summary>
    /// Used to make a new stat from existing stats.
    /// </summary>
    public static DerivedStat FromExisting(DeriveNewStatFromExistingStatsCalculator logic, params Stat.Type[] statTypes) {
      return new DerivedStat(statTypes, sheet => (IStat)logic.DynamicInvoke(sheet, statTypes.Select(s => sheet.Provider.Get(s))));
    }

    /// <summary>
    /// Used to make a new stat from existing stats.
    /// </summary>
    public static DerivedStat ScaledFromExisting<TStat>(params (Stat.Type type, float ratio)[] inputStatRatiosAddingUpTo1)
      where TStat : Stat.Type
        => new(
          inputStatRatiosAddingUpTo1.Select(x => x.type),
          sheet => {
            float value = 0;
            foreach ((Stat.Type type, float scale) in inputStatRatiosAddingUpTo1) {
              value += sheet.Provider.Get(type).CurrentValue * scale;
            }

            //value = andThen?.Invoke(value) ?? value;
            return Stat.Types.Get<TStat>().Make(value);
          }
        );

    /// <summary>
    /// Used to make a new stat from existing stats.
    /// </summary>
    public static DerivedStat ScaledFromExisting<TStat>(Func<float, float> finalizeStatValue, params (Stat.Type type, float ratio)[] inputStatRatiosAddingUpTo1)
      where TStat : Stat.Type
        => new(
          inputStatRatiosAddingUpTo1.Select(x => x.type),
          sheet => {
            float value = 0;
            foreach ((Stat.Type type, float scale) in inputStatRatiosAddingUpTo1) {
              value += sheet.Provider.Get(type).CurrentValue * scale;
            }

            //value = andThen?.Invoke(value) ?? value;
            return Stat.Types.Get<TStat>().Make(finalizeStatValue(value));
          }
        );

    /// <summary>
    /// Used to make a new stat from existing stats.
    /// </summary>
    public static DerivedStat ScaledFromExisting(Func<float, IStat> finalizeStat, params (Stat.Type type, float ratio)[] inputStatRatiosAddingUpTo1)
      => new(
        inputStatRatiosAddingUpTo1.Select(x => x.type),
        sheet => {
          float value = 0;
          foreach ((Stat.Type type, float scale) in inputStatRatiosAddingUpTo1) {
            value += sheet.Provider.Get(type).CurrentValue * scale;
          }

          return finalizeStat(value);
        }
      );
  }
}
