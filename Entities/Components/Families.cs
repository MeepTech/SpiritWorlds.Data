using Meep.Tech.Data;
using System.Collections.Generic;

namespace SpiritWorlds.Data {
  namespace Components {
    public static partial class Entities {

      /// <summary>
      /// A component containing the familiies an entity is from.
      /// </summary>
      public class Families
        : Dictionary<string, Entity.Family>,
          IModel.IComponent<Families>,
          IModel.IComponent.IIsRestrictedTo<Entity>,
          IComponent.IUseDefaultUniverse { }
    }
  }

  public static class EntityFamiliesExtensions {

    /// <summary>
    /// Add the family to the families this entity is a part of.
    /// </summary>
    internal static void _addToFamily(this Entity entity, Entity.Family family) {
      if (entity.TryToGetComponent<Components.Entities.Families>(out var existingComponent)) {
        existingComponent.Add(family.Name, family);
      } else {
        var familyComponent = entity.AddNewComponent<Components.Entities.Families>();
        familyComponent.Add(family.Name, family);
      }
    }
  }
}
