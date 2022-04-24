using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    /// <summary>
    /// An entity that moves around and breathes.
    /// </summary>
    public partial class Denizen
    : Entity {

      protected Denizen(IBuilder<Entity> builder)
        : base(builder) { }
    }
  }
}