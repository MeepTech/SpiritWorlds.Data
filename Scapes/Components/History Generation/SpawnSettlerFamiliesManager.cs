using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;

namespace SpiritWorlds.Data.Components {
  public partial class Scapes {
    public partial class Histories {
      public partial class Generators {

        /// <summary>
        /// A component that can be used to dictate how settler denizen families spawn between ticks.
        /// </summary>
        public class SpawnSettlerFamiliesManager
          : Scape.History.Generator.PerTickManagementComponent<SpawnSettlerFamiliesManager> 
        {

          /// <summary>
          /// max initial settler families to place
          /// </summary>
          public int MaxInitialSettlerFamiliesCount {
            get;
            init;
          } = 15;

          /// <summary>
          /// min initial settler families to place
          /// </summary>
          public int MinInitialSettlerFamiliesCount {
            get;
            init;
          } = 5;

          /// <summary>
          /// max initial settler familiy size
          /// </summary>
          public int MaxInitialSettlerFamililySizeCount {
            get;
            init;
          } = 12;

          /// <summary>
          /// min initial settler familiy size
          /// </summary>
          public int MinInitialSettlerFamililySizeCount {
            get;
            init;
          } = 3;

          /// <summary>
          /// The builder used to make settler families.
          /// </summary>
          public IEnumerable<Data.Entities.Creature.Type> PotentialSettlerTypes {
            get;
          }

          public SpawnSettlerFamiliesManager(IEnumerable<Data.Entities.Creature.Type> potentialSettlerTypes) {
            PotentialSettlerTypes = potentialSettlerTypes;
          }

          /*SpawnSettlerFamiliesManager(IBuilder builder) 
            : base(builder) {
              PotentialSettlerTypes 
                = builder.GetAndValidateParamAs<IEnumerable<Data.Entities.Creature.Type>>(nameof(PotentialSettlerTypes));
              FamilyBuilder 
                = builder.GetParam<Entity.Family.Type>(
                  nameof(FamilyBuilder),
                  Entity.Family.Type.Base.Archetype as Entity.Family.Type
                );
          } SpawnSettlerFamiliesManager() 
            : base() { }*/

          protected internal override Scape.History ProcessForCurrentTick(Scape.History currentHistory, Scape.Moment.Delta? timeSinceLastTick) {
            // initalization:
            if (currentHistory.CurrentMoment == currentHistory.FirstMoment) {
              Dictionary<string, Entity.Family> families = new();
              // get how many families we should initially spawn at world creation
              for (
                int familyIndex = 0;
                familyIndex < currentHistory.Scape.SeedBasedRandomizer.Next(MinInitialSettlerFamiliesCount, MaxInitialSettlerFamiliesCount);
                familyIndex++
              ) {
                // make each family
                Entity.Family family = _generateNewSettlerFamily(currentHistory);
                families.Add(family.Name, family);
              }
            } 
            
            // after that, small random spawn chances every few years:
            else {

            }

            return currentHistory;
          }

          Entity.Family _generateNewSettlerFamily(Scape.History currentHistory) {
            /// make a new family name.
            string familyName = Meep.Tech.Noise.RNG.GenerateRandomNewWord(
              currentHistory.Scape.SeedBasedRandomizer.Next(4, 12),
              currentHistory.Scape.SeedBasedRandomizer
            );

            // generate the family members.
            Entity.Family family = (Entity.Family.Type.Base.Archetype as Entity.Family.Type).Make(familyName);
            for (
              int memberOfTheFamilyIndex = 0;
              memberOfTheFamilyIndex < currentHistory.Scape.SeedBasedRandomizer.Next(MinInitialSettlerFamililySizeCount, MaxInitialSettlerFamililySizeCount);
              memberOfTheFamilyIndex++
            ) {
              Entity newFamilyMember = PotentialSettlerTypes
                .RandomEntry(currentHistory.Scape.SeedBasedRandomizer)
                .Make(
                  (
                    nameof(Entity.Name),
                    Meep.Tech.Noise.RNG.GenerateRandomNewWord(
                      currentHistory.Scape.SeedBasedRandomizer.Next(4, 12),
                      currentHistory.Scape.SeedBasedRandomizer
                    )
                  ),
                  (
                    nameof(Data.Entities.Denizen.Surname),
                    familyName
                  ),
                  (
                    nameof(Data.Components.Entities.History.MomentOfCreation),
                    new Scape.Moment (currentHistory.CurrentMoment.ValueInYears
                        // TODO: add age randomizer options.
                        - (currentHistory.Scape.SeedBasedRandomizer.Next(800, 6500) / 100.0f)
                    )
                  )
                );

              if (family.RootMember is not null) {
                family.AddFamilyMember(newFamilyMember);
              }
              else {
                family.Start(newFamilyMember);
              }
            }

            return family;
          }
        }
      }
    }
  }
}