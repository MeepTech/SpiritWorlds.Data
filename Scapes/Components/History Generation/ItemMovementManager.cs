using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;

namespace SpiritWorlds.Data.Components {
  public partial class Scapes {
    public partial class Histories {
      public partial class Generators {

        /// <summary>
        /// A component that can be used to dictate how certain items move around the map between tics.
        /// </summary>
        public class ItemMovementManager
          : Scape.History.Generator.PerTickManagementComponent<ItemMovementManager> 
        {

          ItemMovementManager(IBuilder builder) 
            : base(builder) { } ItemMovementManager() 
            : base() { }

          protected internal override Scape.History ProcessForCurrentTick(Scape.History currentHistory) {
            throw new System.NotImplementedException();
          }
        }
      }
    }
  }
}