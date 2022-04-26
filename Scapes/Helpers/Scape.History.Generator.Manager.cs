using Meep.Tech.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
/* 
namespace SpiritWorlds.Data {
  public partial class Scape {
   public partial class History {
      public partial class Generator {

        /// <summary>
        /// Used to run and manage multuple history generators.
        /// </summary>
        public static class Manager {

          /// <summary>
          /// Generate a history with a set of generators.
          /// </summary>
          public static History Run(History onHistory, float forLengthOfTime_inYears, params Generator[] withGenerators)
            => Run(onHistory, forLengthOfTime_inYears, withGenerators as IEnumerable<Generator>);

          /// <summary>
          /// Generate a history with a series of eras, each with a specific set of generators.
          /// </summary>
          public static History Run(History onHistory, params Era[] withEras)
            => Run(onHistory, withEras as IEnumerable<Era>);

          /// <summary>
          /// Generate a history with a set of generators.
          /// </summary>
          public static History Run(History onHistory, float forLengthOfTime_inYears, float withTickLength_inYears, params Generator[] withGenerators)
            => Run(onHistory, forLengthOfTime_inYears, withGenerators, withTickLength_inYears);

          /// <summary>
          /// Generate a history with a series of eras, each with a specific set of generators.
          /// </summary>
          public static History Run(History onHistory, float withTickLength_inYears, params Era[] withEras)
            => Run(onHistory, withEras, withTickLength_inYears);

          /// <summary>
          /// Generate a history with a set of generators.
          /// </summary>
          public static History Run(History onHistory, double forLengthOfTime_inYears, IEnumerable<Generator> withGenerators, Scape.Moment.Delta withTickLength = 1) {
            for (double currentYear = onHistory.FirstMoment.ValueInYears; currentYear < forLengthOfTime_inYears; currentYear += withTickLength.InYears) {
              // process global plots
              foreach (Generator generator in withGenerators) {
                generator._processTick(onHistory, withTickLength);
              }

              onHistory._currentMoment.ValueInYears = currentYear;
            }

            return onHistory;
          }

          /// <summary>
          /// Generate a history with a series of eras, each with a specific set of generators.
          /// </summary>
          public static History Run(History onHistory, IEnumerable<Era> withEras, double withTickLength_inYears = 1) {
            withEras.ForEach(e => {
              e._begin(onHistory);
              Run(onHistory, e.Length, e.Generators, withTickLength_inYears);
              e._end(onHistory);
            });

            return onHistory;
          }
        }
      }
    }
  }
}
   */