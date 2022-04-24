using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;

namespace SpiritWorlds.Data {
  public static partial class Inventories {

    /// <summary>
    /// Just a set of items.
    /// </summary>
    public sealed partial class BasicInventory : Inventory {

      /// <summary>
      /// Make an empty basic inventory
      /// </summary>
      public BasicInventory()
        : base(null) { }

      /// <summary>
      /// Make a basic inventory from a set of item stacks
      /// </summary>
      public BasicInventory(IEnumerable<ValueStack<Item>> stacks)
        : this() { LoadFromStacks(stacks); }

      /// <summary>
      /// Make a basic inventory from a set of items
      /// </summary>
      public BasicInventory(IEnumerable<Item> items)
        : this() { LoadFromItems(items); }

      /// <summary>
      /// Xbam
      /// </summary>
      BasicInventory(IBuilder builder)
        : base(builder) { }

      ///<summary><inheritdoc/></summary>
      public override bool Add(ValueStack<Item> items, out int remainder) {
        Values.Add(items);
        remainder = 0;
        return true;
      }
    }
  }
}
