using Meep.Tech.Data;
using System;

namespace SpiritWorlds.Data {

  public partial class PlotTree {

    public partial class Node {
      /// <summary>
      /// The Base Archetype for Nodes
      /// </summary>
      public abstract class Type : Archetype<Node, Node.Type> {

        /// <summary>
        /// Used to make new Child Archetypes for Node.Type 
        /// </summary>
        /// <param name="id">The unique identity of the Child Archetype</param>
        protected Type(Identity id)
          : base(id) { }

        /// <summary>
        /// Calculate the a chance that the parent plot tree will be forced to progress onward from this node. 0 is never, 1 always happens.
        /// </summary>
        /// <returns> A value between 0 and 1 that will be compared to a random rolled value. If the random value is less, then progression occurs.</returns>
        protected internal abstract float CalculateProgressionChance(Scape.History currentHistory);

        /// <summary>
        /// Calculate the a chance that this node will be chosen next or that this plot tree will be assigned by the generator (if this is a root node). 0 is never, 1 always happens.
        /// </summary>
        /// <returns> A value between 0 and 1 that will be compared to a random rolled value.</returns>
        protected internal abstract float CalculateChanceAsNextChoice(Scape.History currentHistory);
      }
    }
  }
}