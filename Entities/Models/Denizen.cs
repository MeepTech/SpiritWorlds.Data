using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    /// <summary>
    /// An entity that moves around and breathes.
    /// </summary>
    public partial class Denizen
      : Creature {

      /// <summary>
      /// The family name of this Denizen.
      /// </summary>
      public virtual string Surname { 
        get; 
        protected set;
      }

      protected Denizen(IBuilder<Entity> builder)
        : base(builder) { }
    }
  }
}