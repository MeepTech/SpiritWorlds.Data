using System.Collections.Generic;

namespace SpiritWorlds.Data {
  public static partial class Entities {
    public partial struct Tile {
      public partial class Feature {

        /// <summary>
        /// A type of tile feature
        /// </summary>
        public new abstract class Type : Entity.Type {

          /// <summary>
          /// The base durability value of this feature.
          /// </summary>
          public float BaseDurability {
            get;
          } = 1000;

          /// <summary>
          /// The base durability value of this feature.
          /// </summary>
          public IReadOnlyDictionary<Components.Items.Tool.Type, float> ToolEffectivenessMultupliers {
            get;
          }

          /// <summary>
          /// If this type of tile feature is a decoration.
          /// Decorations can overlay other features.
          /// </summary>
          public bool IsDecoration {
            get;
          } = false;

          /// <summary>
          /// If this type of tile feature allows decoration features to be overlayed on the same layer.
          /// </summary>
          public bool AllowDecorationOverlays {
            get;
          } = true;

          protected Type(Identity id, bool isDecoration = false, bool allowDecorationOverlays = true, float baseDurability = 1000, Dictionary<Components.Items.Tool.Type, float> toolEffectivnesMultipliers = null)
            : base(id) {
            IsDecoration = isDecoration;
            AllowDecorationOverlays = allowDecorationOverlays;
            BaseDurability = baseDurability;
            ToolEffectivenessMultupliers = toolEffectivnesMultipliers ?? new(); 
          }
        }
      }
    }
  }
}
