using System;
using System.Collections.Generic;
using System.Linq;
using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using SpiritWorlds.Data.Components;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace SpiritWorlds.Data {
  public static partial class Entities {
    public partial class Creature {

      /// <summary>
      /// The Base Archetype for Creatures
      /// </summary>
      [Branch]
      public new abstract class Type : Entity.Type {

        protected override HashSet<System.Type> InitialUnlinkedModelComponentTypes
          => _InitialUnlinkedModelComponentTypes
            ??= base.InitialUnlinkedModelComponentTypes
              .Append(typeof(Components.Entities.CoreStats));

        protected override Dictionary<string, IComponent> InitialComponents
          => _InitialComponents
            ??= base.InitialComponents
              .Append(LevelUpSettings.Make(
                getExpRequiredToLevelUpTo: level => Math.Round(0.04 * Math.Pow(level, 3) + 0.8 * Math.Pow(level, 2) + 2 * level)
              ).Modify(settings 
                => settings.DefaultOnLevelUpSuccessActions.Add(
                  "Core Stat Level Up", 
                  (stats, levels) => {
                    Components.Entities.CoreStats coreStats = settings.LinkedModelComponent.Parent
                      .GetComponent<Components.Entities.CoreStats>();

                    Stat.Type coreStatToUpdate = stats.FirstOrDefault(stat => coreStats.Contains(stat));
                    if (coreStatToUpdate is not null) {
                      coreStats.ModifyBase(coreStatToUpdate, coreStats.Get(coreStatToUpdate).BaseValue + 1, out _);
                    }
                  }
                )
              ));

        /// <summary>
        /// Used to make new Child Archetypes for Creature.Type 
        /// </summary>
        /// <param name="id">The unique identity of the Child Archetype</param>
        protected Type(Identity id)
          : base(id) { }
      }
    }
  }
}
