using System;

namespace SpiritWorlds.Data {

  public partial class PlotTree {
    public partial class GlobalLevel {
      [Meep.Tech.Data.Configuration.Loader.Settings.Branch]
      public abstract new class Type : PlotTree.Type {
        protected Type(Identity id)
          : base(id) { }

        public PlotTree.GlobalLevel Make(Scape.History.Generator generator, Scape.History currentHistory) {
          throw new NotImplementedException();
        }
      }
    }
  }
}
