namespace SpiritWorlds.Data {

  public partial class PlotTree {
    public partial class GlobalLevel {
      public new abstract class Node : PlotTree.Node.Type {
        protected Node(Identity id)
          : base(id) { }
      }
    }
  }
}
