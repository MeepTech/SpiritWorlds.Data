using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;

namespace SpiritWorlds.Data.Components {
  public partial class Scapes {
    public partial class Histories {
      public partial class Generators {

        /// <summary>
        /// A component that can be used to dictate how npcs move around the map between tics.
        /// </summary>
        public class NpcMovementManager
          : Scape.History.Generator.PerTickManagementComponent<NpcMovementManager> 
        {

          NpcMovementManager(IBuilder builder) 
            : base(builder) { } NpcMovementManager() 
            : base() { }

          protected internal override Scape.History ProcessForCurrentTick(Scape.History currentHistory, Scape.Moment.Delta? timeSinceLastTick) {
            throw new System.NotImplementedException();
          }
        }
      }
    }
  }
}