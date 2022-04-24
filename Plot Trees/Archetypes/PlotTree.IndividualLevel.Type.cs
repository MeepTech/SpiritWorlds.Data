using System;

namespace SpiritWorlds.Data {

  public partial class PlotTree {
    public partial class IndividualLevel {
      [Meep.Tech.Data.Configuration.Loader.Settings.Branch]
      public abstract new class Type : PlotTree.Type {
        protected Type(Identity id)
          : base(id) { }

        public IndividualLevel Make(Entity assignedNpc, Scape.History.Generator generator, Scape.History currentHistory) {
          throw new NotImplementedException();
        }
      }
    }
  }
}
