using Meep.Tech.Data;

namespace SpiritWorlds.Data {

  public partial class Inventory {
    /// <summary>
    /// The Base Archetype for Inventorys
    /// </summary>
    public abstract class Type : Archetype<Inventory, Inventory.Type> {

      /// <summary>
      /// A general description of this type
      /// </summary>
      public virtual string Description {
        get;
      } = "An Inventory of Items";

      /// <summary>
      /// Used to make new Child Archetypes for Inventory.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }
    }
  }
}
