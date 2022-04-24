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
          public virtual int MaxInitialSettlerFamiliesCount {
            get;
          }

          /// <summary>
          /// min initial settler families to place
          /// </summary>
          public virtual int MinInitialSettlerFamiliesCount {
            get;
          }

          /// <summary>
          /// max initial settler familiy size
          /// </summary>
          public virtual int MaxInitialSettlerFamililySizeCount {
            get;
          }

          /// <summary>
          /// min initial settler familiy size
          /// </summary>
          public virtual int MinInitialSettlerFamililySizeCount {
            get;
          }

          /// <summary>
          /// Initial settler families to place
          /// </summary>
          public virtual int InitialSettlerFamiliesCount {
            get;
          }

          /// <summary>
          /// The builder used to make settler families.
          /// </summary>
          public Entity.Family.Type FamilyBuilder {
            get;
          }

          SpawnSettlerFamiliesManager(IBuilder builder) 
            : base(builder) {
            FamilyBuilder = builder.GetParam<Entity.Family.Type>(nameof(FamilyBuilder));
          } SpawnSettlerFamiliesManager() 
            : base() { }

          protected internal override Scape.History ProcessForCurrentTick(Scape.History currentHistory) {
            // initalization:
            if (currentHistory.CurrentMoment == currentHistory.FirstMoment) {
              Dictionary<string, IEnumerable<Entity>> families = new();
              // get how many families we should initially spawn at world creation
              for (
                int familyIndex = 0;
                familyIndex < currentHistory.Scape.SeedBasedRandomizer.Next(MinInitialSettlerFamiliesCount, MaxInitialSettlerFamiliesCount);
                familyIndex++
              ) {
                // make each family
                string familyName = Meep.Tech.Noise.RNG.GenerateRandomNewWord(
                  currentHistory.Scape.SeedBasedRandomizer.Next(4, 12),
                  currentHistory.Scape.SeedBasedRandomizer
                );

                // generate the family members.
                Entity.Family family = FamilyBuilder.Make(familyName);
                for (
                  int memberOfTheFamilyIndex = 0;
                  memberOfTheFamilyIndex < currentHistory.Scape.SeedBasedRandomizer.Next(MinInitialSettlerFamililySizeCount, MaxInitialSettlerFamililySizeCount);
                  memberOfTheFamilyIndex++
                ) {
                  family.GenerateNewMember(currentHistory.Scape.SeedBasedRandomizer);
                }

                // collect all the families and members
                families.Add(familyName, family);
              }
            } // after that, small random spawn chances:
            else {

            }

            return currentHistory;
          }
        }
      }
    }
  }
}