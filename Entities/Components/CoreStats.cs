using Meep.Tech.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data.Components {
  public static partial class Entities {

    /// <summary>
    /// A component containing the basic core entity stats used in spiritworlds.
    /// </summary>
    public class CoreStats
    : Stat.Sheet,
      IModel.IComponent.IIsRestrictedTo<Entity>,
      IModel.IComponent<CoreStats>,
      IComponent.IUseDefaultUniverse {

      /// <summary>
      /// Load this stat sheet using the code stats provided.
      /// </summary>
      public static CoreStats Make(float ED = 1, float PO = 1, float VZ = 1, float FN = 1)
        => Components<CoreStats>.BuilderFactory.Make((nameof(Stats), Data.Stats.Sheets.CoreStats.Make(ED, PO, VZ, FN)));

      #region XBam Config 

      CoreStats(IBuilder builder)
        : base(builder.GetParam<IEnumerable<IStat>>(nameof(Stats)).Where(
          stat => Stat.Sheet.Types.Get<Stats.Sheets.CoreStats>()
            .DefaultBaseStatTypes.Contains(stat.Archetype))
          .ToHashSet()
        ) { }
      CoreStats(HashSet<IStat> stats) : base(stats) { }

      JObject IModel.ToJson()
        => (this as IModel.IComponent).ToJson();

      #endregion
    }
  }
}