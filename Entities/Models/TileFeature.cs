using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public static partial class Entities {
    public partial struct Tile {

      /// <summary>
      /// A fixed feature on a tile.
      /// </summary>    
      public partial class Feature : Entity {

        /// <summary>
        /// The durability stat of this feature.
        /// </summary>
        public DepleteableStat Durability
          => _durability ??= Stat.Types.Get<Stats.Types.Durability>()
            .MakeDepleteable((Archetype as Entities.Tile.Feature.Type).BaseDurability);
        DepleteableStat? _durability;

        /// <summary>
        /// The size/amount of the tile this feature takes up.
        /// </summary>
        public enum Size {
          Small,
          Full
        }

        /// <summary>
        /// The possible layers a feature can be added on
        /// </summary>
        public enum Layer {
          Ground,
          Sky
        }

        protected Feature(IBuilder<Entity> builder)
          : base(builder) {}
      }
    }
  }
}
