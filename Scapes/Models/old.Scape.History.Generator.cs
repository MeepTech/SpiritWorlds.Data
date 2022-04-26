using Meep.Tech.Data;
using Meep.Tech.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

/*namespace SpiritWorlds.Data {
  public partial class Scape {
    public partial class History {

      /// <summary>
      /// Generators are used to build Eras and chunks of history by advancing and assigning plot trees.
      /// </summary>
      //public partial class Generator : Model<Generator, Generator.Type>.WithComponents {

        /*List<PlotTree.GlobalLevel.Type> _potentialGlobalTrees = new();
        List<PlotTree.IndividualLevel.Type> _potentialNpcTrees = new();
        List<(PlotTree.IndividualLevel.Type, float)> _potentialNpcTreesThatHaveMetTheThreshold = new();
        Dictionary<PlotTree.Type, int> _potentialTreeCounts = new();*/
        /*Generator() {}

        internal History _processTick(History currentHistory, Scape.Moment.Delta? timeSinceLastTick) {
          foreach (PerTickManagementComponent component in Components.Values) {
            component.ProcessForCurrentTick(currentHistory, timeSinceLastTick);
          }

          return currentHistory;
        }*/

        /*internal History _processTickForNPC(Entity npc, History currentHistory) {
          // try to start global trees:
          foreach ((PlotTree.IndividualLevel.Type npcPlotType, float globalCalculatedChanceValue) in _potentialNpcTreesThatHaveMetTheThreshold) {
            if ((npcPlotType.RootNodeType as PlotTree.IndividualLevel.Node).CalculateRootNodeChanceToAssignToNpc(npc, currentHistory, globalCalculatedChanceValue) >= currentHistory.Scape.SeedBasedRandomizer.NextDouble()) {
              // build the new tree and add it to the active plots 
              currentHistory._activePlotTrees.Add(Archetype.PlotTreeInitializationOverrides.TryGetValue(npcPlotType, out var initializer)
                ? initializer(npcPlotType, this, currentHistory.ActivePlotTrees.Where(t => t.Archetype == npcPlotType).Count())
                : npcPlotType.Make(npc, this, currentHistory));

              // tick down the count of this type of tree
              _potentialTreeCounts[npcPlotType] -= 1;
            }
          }

          return currentHistory;
        }*/

        /*/// <summary>
        /// A type of plot tree
        /// </summary>
        public partial class Type : Archetype<Generator, Generator.Type> {

          protected Type(Identity id)
            : base(id) { }
        }
      }
    }
  }
}
*/