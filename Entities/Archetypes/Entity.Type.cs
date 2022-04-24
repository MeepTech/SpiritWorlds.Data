using Meep.Tech.Data;
using System.Collections.Generic;

namespace SpiritWorlds.Data {

  public partial class Entity {

    /// <summary>
    /// The Base Archetype for Entitys
    /// </summary>
    public abstract class Type : Archetype<Entity, Entity.Type>.WithDefaultBuilderBasedModelBuilders {

      /// <summary>
      /// A general description of this type
      /// </summary>
      public abstract string Description {
        get;
      }

      /// <summary>
      /// Used to make new Child Archetypes for Entity.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }
    }
  }
}
