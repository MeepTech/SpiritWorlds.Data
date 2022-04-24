using Meep.Tech.Data;
using System;
using System.Collections.Generic;

namespace SpiritWorlds.Data {
  public partial class Scape {
    public partial class History {

      /// <summary>
      /// Eras are chunks of time in the history that can be built with sets of plot trees and other generator elements
      /// </summary>
      public partial class Era : Model<Era, Era.Type> {

        /// <summary>
        /// How many years long this era is.
        /// </summary>
        public float Length {
          get;
        }

        /// <summary>
        /// The different generators run per tick to build this era.
        /// </summary>
        public IEnumerable<Generator> Generators {
          get;
        }

        internal void _begin(History onHistory) {
          // adds the era to the current history, sets it to the current era. Makes all logs taged with the current era
          throw new NotImplementedException();
        }

        internal void _end(History onHistory) {
          //ends and seals this era in world history. ends log tagging to this era
          throw new NotImplementedException();
        }
      }
    }
  }
}
