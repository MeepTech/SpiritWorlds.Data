namespace SpiritWorlds.Data {
  public static partial class Stats {
    public partial class Types {
      /// <summary>
      /// How durable something is.
      /// Replaces health in inanimate objects.
      /// </summary>
      public class Durability : Stat.Type {
        Durability()
          : base(
            "Tile.Feature.",
            nameof(Durability),
            "DUR",
            "How durable this object is. How much damage it can take before it breaks, or potentially de-levels",
            10000
          ) { }
      }
    }
  }
}