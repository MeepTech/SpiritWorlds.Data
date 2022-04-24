using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using SpiritWorlds.Data.Components;
using System;
using System.Collections.Generic;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    public partial class Monster {

      /// <summary>
      /// The Base Archetype for Monsters
      /// </summary>
      [Branch]
      public new abstract class Type : Creature.Type {

        protected override HashSet<System.Type> InitialUnlinkedModelComponentTypes
          => _InitialUnlinkedModelComponentTypes
            ??= base.InitialUnlinkedModelComponentTypes
              .Append(typeof(Components.Entities.CombatStats));

        /// <summary>
        /// Used to make new Child Archetypes for Monster.Type 
        /// </summary>
        /// <param name="id">The unique identity of the Child Archetype</param>
        protected Type(Identity id)
          : base(id) { }
      }
    }
  }
}