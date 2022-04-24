using Meep.Tech.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpiritWorlds.Data.Components {

  /// <summary>
  /// Static item components data.
  /// </summary>
  public static partial class Items {

    /// <summary>
    /// Marks an item as a tool.
    /// Tools can do extra damage to Tile Features.
    /// Some tools can be leveld up.
    /// </summary>
    public partial class Tool
      : IComponent.IUseDefaultUniverse,
        IModel.IComponent<Tool>,
        IModel.IComponent.IIsRestrictedTo<Item>,
        IModel.IComponent<Tool>.IHasContractWith<Levels> {

      /// <summary>
      /// The Types for this tool
      /// </summary>
      public IEnumerable<Tool.Type> Types {
        get;
      }

      /// <summary>
      /// The level data of this tool.
      /// This is a link to the levels component if it exists.
      /// </summary>
      [JsonIgnore]
      public Levels Levels {
        get;
        private set;
      }

      #region XBam Configuration

      Tool(IBuilder<Tool> builder) {
        Types = (builder.Parent as Item).Archetype
          .TryToGetComponent<ToolSettings>(out var found)
           ? found.DefaultTypes
           : null;
      }

      (Tool @this, Levels other) IComponent<Tool>.IHasContractWith<Levels>.ExecuteContractWith(Levels otherComponent) {
        Levels = otherComponent;

        return (this, otherComponent);
      }

      #endregion
    }
  }
}
