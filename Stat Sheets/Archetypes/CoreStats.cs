using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {
  public static partial class Stats {
    public static partial class Sheets {
      public class CoreStats : Stat.Sheet.Type {
        protected CoreStats()
          : base(
            new Identity("Core"),
            "The core stats that most other stats derive from",
            defaultFixedStats: new Stat.Type[] {
              Stat.Types.Get<Potency>(),
              Stat.Types.Get<Endurance>(),
              Stat.Types.Get<Vision>(),
              Stat.Types.Get<Finesse>()
            }.ToHashSet()
          ) { }

        /// <summary>
        /// Used to make a new default core stat sheet.
        /// </summary>
        public static Stat.Sheet Make(float ED = 1, float PO = 1, float VZ = 1, float FN = 1)
          => Stat.Sheet.Types
            .Get<CoreStats>()
              .Make<Stat.Sheet>((nameof(Stat.Sheet.BaseStats), new Dictionary<Stat.Type, float> {
                {Stat.Types.Get<Endurance>(), ED },
                {Stat.Types.Get<Potency>(), PO },
                {Stat.Types.Get<Vision>(), VZ },
                {Stat.Types.Get<Finesse>(), FN },
              }));

        /// <summary>
        /// Make helper
        /// </summary>
        internal new Stat.Sheet Make(IBuilder builder)
          => Make<Stat.Sheet>(builder);
      }
    }
  }
}
