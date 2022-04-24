using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace SpiritWorlds.Data {
  public static partial class Entities {

    public partial class PlayerCharacter {

      /// <summary>
      /// The Base Archetype for People
      /// </summary>
      [Branch]
      public new abstract class Type : Person.Type {
        protected override Dictionary<string, IComponent> InitialComponents
          => _InitialComponents
            ??= base.InitialComponents
              .Update<Data.Components.LevelUpSettings>(component =>
                component.CopyWithNewBaseFunctionality(
                  newGetRequiredLevelUpExpFunction: level => 
                    (double)Math.Round((decimal)((4 * (level ^ 3)) / 5))));

        /// <summary>
        /// Used to make new Child Archetypes for Person.Type 
        /// </summary>
        /// <param name="id">The unique identity of the Child Archetype</param>
        protected Type(Identity id)
          : base(id) { }
      }
    }
  }
}