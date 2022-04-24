namespace SpiritWorlds.Data {

  public static partial class PlotTrees {

    /// <summary>
    /// General tags for plot trees
    /// </summary>
    public static partial class Tags {

      /// <summary>
      /// Signifies a plot tree that is owned by the world
      /// </summary>
      public static Tag GlobalyOwned {
        get;
      } = new Tag("global-plot-tree");

      /// <summary>
      /// Signifies a plot tree owned by an individual npc
      /// </summary>
      public static Tag NpcOwned {
        get;
      } = new Tag("npc-plot-tree");

      /// <summary>
      /// Signifies the owner of the plot tree.
      /// Use with extra context being the id of the thing that owns this plot tree.
      /// </summary>
      public static Tag Owner {
        get;
      } = new Tag("plot-tree-owner");

      /// <summary>
      /// Signifies the type of the plot tree.
      /// Use with extra context being the id of the thing that owns this plot tree.
      /// </summary>
      public static Tag Type {
        get;
      } = new Tag("plot-tree-type");

      /// <summary>
      /// Signifies the id of the plot tree.
      /// Use with extra context being the id of the thing that owns this plot tree.
      /// </summary>
      public static Tag Id {
        get;
      } = new Tag("plot-tree-id");
    }
  }
}
