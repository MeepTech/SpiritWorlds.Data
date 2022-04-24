namespace SpiritWorlds.Data {

  public partial class Biome {
    public partial class Feature {
      /// <summary>
      /// The Base Archetype for Features
      /// </summary>
      public new abstract class Type : Board.Feature.Type {

        /// <summary>
        /// Used to make new Child Archetypes for Feature.Type 
        /// </summary>
        /// <param name="id">The unique identity of the Child Archetype</param>
        protected Type(Identity id)
          : base(id) { }
      }
    }
  }
}
