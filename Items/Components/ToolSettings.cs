using Meep.Tech.Data;
using System.Collections.Generic;

namespace SpiritWorlds.Data.Components {
  public static partial class Items {

    /// <summary>
    /// Settings for a tool produced by an archetype.
    /// </summary>
    public class ToolSettings
    : Archetype.IComponent<ToolSettings>,
      Archetype.IComponent.IIsRestrictedTo<Item.Type>,
      Archetype.ILinkedComponent<Tool>,
      IComponent.IUseDefaultUniverse {

      /// <summary>
      /// The types of tool this uses.
      /// </summary>
      public IEnumerable<Tool.Type> DefaultTypes {
        get;
        protected set;
      }
    }
  }
}