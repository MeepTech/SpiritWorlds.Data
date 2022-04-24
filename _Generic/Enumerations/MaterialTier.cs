using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A tier of a thing, based on it's materials.
  /// </summary>
  public class MaterialTier : Enumeration<MaterialTier> {

    /// <summary>
    /// Associated materials with this tier in order of importance.
    /// </summary>
    public IReadOnlyList<Item.Type> AssociatedMaterials
      { get; }

    /// <summary>
    /// Make a new tier of materials to use in things like items and entities
    /// </summary>
    /// <param name="uniqueNameForTier">The teir's unique name</param>
    /// <param name="associatedMaterials">Associated materials with this tier in order of importance.</param>
    protected MaterialTier(string prefix, string uniqueNameForTier, IList<Item.Type> associatedMaterials = null)
      : base(prefix + uniqueNameForTier) {
      AssociatedMaterials = associatedMaterials?.ToList() ?? new();
    }
  }
}