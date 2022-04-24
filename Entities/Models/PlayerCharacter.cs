using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    /// <summary>
    /// A player character
    /// </summary>
    public partial class PlayerCharacter : Person {

      protected PlayerCharacter(IBuilder<Entity> builder)
        : base(builder) { }
    }
  }
}
