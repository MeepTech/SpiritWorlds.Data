using Meep.Tech.Data;
using System;
using System.Collections.Generic;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    /// <summary>
    /// A Creature that can fight, and maybe even talk.
    /// </summary>
    public partial class Monster : Creature {

      protected Monster(IBuilder<Entity> builder)
        : base(builder) { }
    }
  }

}