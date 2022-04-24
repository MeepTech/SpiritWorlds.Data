using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;

namespace SpiritWorlds.Data.Components {
  public static partial class Entities {

    /// <summary>
    /// A component containing the basic combat stats used in spiritworlds.
    /// </summary>
    [Loader.Settings.Dependency(typeof(Stats.Sheets.CoreStats))]
    public abstract class CombatStats
    : IModel.IComponent<CombatStats>,
      IModel.IComponent.IIsRestrictedTo<Entity>,
      IComponent.IUseDefaultUniverse {

      /// <summary>
      /// Combat resource stats
      /// </summary>
      public abstract Stat.Sheet Resources {
        get;
      }

      /// <summary>
      /// Offencive combat stats
      /// </summary>
      public abstract Stat.Sheet DamageMultipliers {
        get;
      }

      /// <summary>
      /// Defensive combat stats
      /// </summary>
      public abstract Stat.Sheet DamageResistances {
        get;
      }

      /// <summary>
      /// Defensive combat stats
      /// </summary>
      public abstract Stat.Sheet StatusEffectResistances {
        get;
      }

      protected CombatStats(IBuilder<CombatStats> builder) {
        Stat.Sheet coreStats
          = builder.GetAndValidateParamAs<Stat.Sheet>(nameof(Stats.Sheets.CoreStats));
        Resources.Dependencies.Add(coreStats.Archetype, coreStats);
        DamageMultipliers.Dependencies.Add(coreStats.Archetype, coreStats);
        DamageResistances.Dependencies.Add(coreStats.Archetype, coreStats);
        StatusEffectResistances.Dependencies.Add(coreStats.Archetype, coreStats);
      }

      #region Xbam Configuration

      internal static Stat.Sheet _testCoreStats
        = Stats.Sheets.CoreStats.Make();

      static CombatStats() {
        Components<CombatStats>.BuilderFactory
          = new IComponent<CombatStats>.BuilderFactory(new Archetype<CombatStats, IComponent<CombatStats>.BuilderFactory>.Identity("Combat Stats")) {
            DefaultTestParams = new() {
              { nameof(Stats.Sheets.CoreStats), _testCoreStats }
            }
          };
      }

      #endregion
    }
  }
}