using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    public partial class Denizen {

      /// <summary>
      /// The Base Archetype for Denizens (NPC People)
      /// </summary>
      [Branch]
      public new abstract class Type : Person.Type {

        /// <summary>
        /// Used to make new Child Archetypes for Person.Type 
        /// </summary>
        /// <param name="id">The unique identity of the Child Archetype</param>
        protected Type(Identity id)
          : base(id) { }
      }
    }
  }
}
