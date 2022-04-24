using Meep.Tech.Data;
using System;

namespace SpiritWorlds.Data {

  public partial class Item {
    /// <summary>
    /// The Base Archetype for Items
    /// </summary>
    public abstract class Type : Archetype<Item, Item.Type>.WithDefaultParamBasedModelBuilders {

      /// <summary>
      /// The max amount of items of this type allowed in a stack. Null is infinite.
      /// </summary>
      public virtual int? MaxQuantityPerStack {
        get;
      } = null;

      /// <summary>
      /// A general description of this type of item
      /// </summary>
      public abstract string Description {
        get;
      }

      /// <summary>
      /// Used to make new Child Archetypes for Item.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }

      /// <summary>
      /// Used to get the stack key of an item of this type.
      /// </summary>
      public virtual string GetStackKey(Item item)
        => Id.Key + item.Name;
    }
  }
}
