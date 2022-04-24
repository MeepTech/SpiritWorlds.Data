using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    /// <summary>
    /// An entity that moves around and breathes.
    /// </summary>
    public partial class Creature
      : Entity {

      protected Creature(IBuilder<Entity> builder)
        : base(builder) { }
    }
  }
}
