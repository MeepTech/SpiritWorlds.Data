using Meep.Tech.Collections.Generic;
using System;
using System.Collections.Generic;

namespace SpiritWorlds.Data {

  public partial class PlotTree {
    public partial class IndividualLevel {

      /// <summary>
      /// A node of an individual level/npc plot tree
      /// </summary>
      public new abstract class Node : PlotTree.Node.Type {
        protected Node(Identity id)
          : base(id) { }

        /// <summary>
        /// Get valid npcs (if any) from.
        /// </summary>
        protected internal abstract bool TryToFindValidPotentialNpcsForAssignment(Scape.History currentHistory, out IReadOnlyTagedCollection<Tag, Entity> foundValidPotentialNpcs);

        /// <summary>
        /// Tries to assign this node's tree to a random valid potential npc returned from GetValidPotentialNpcsForAssignment.
        /// </summary>
        protected internal abstract bool TryToAssignToAValidPotentialNpc(IReadOnlyTagedCollection<Tag, Entity> npcs, Scape.History currentHistory, out Entity assignedNpc);
      }
    }
  }
}
