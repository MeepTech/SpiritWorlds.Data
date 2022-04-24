using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public static partial class Entities {
    public partial class Person : Creature {

      protected Person(IBuilder<Entity> builder)
        : base(builder) { }
    }
  }
}
