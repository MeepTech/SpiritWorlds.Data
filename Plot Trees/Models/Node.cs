using Meep.Tech.Data;

namespace SpiritWorlds.Data {

  public partial class PlotTree {
    /// <summary>
    /// The Base Model for all Nodes
    /// </summary>
    public partial class Node : Model<Node, Node.Type>, IModel.IUseDefaultUniverse {

      /// <summary>
      /// Calculate the a chance that the parent plot tree will be forced to progress onward from this node. 0 is never, 1 always happens.
      /// </summary>
      /// <returns> A value between 0 and 1 that will be compared to a random rolled value. If the random value is less, then progression occurs.</returns>
      /// <seealso cref="Type.CalculateProgressionChance(Scape.History)"/>
      public virtual float CalculateProgressionChance(Scape.History history, Scape.History.Generator generator)
        => Archetype.CalculateProgressionChance(history);
    }
  }
}