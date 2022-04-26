using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data.Components {
  public partial class Scapes {
    public partial class Histories {
      public partial class Generators {

        /// <summary>
        /// A component that can be used to dictate how npcs are asigned plots between tics.
        /// </summary>
        public class NpcPlotTreeManager
          : Scape.History.Generator.PerTickManagementComponent<NpcPlotTreeManager> {
          List<PlotTree.IndividualLevel.Type> _potentialTrees = new();
          Dictionary<PlotTree.Type, int> _potentialTreeCounts = new();

          public class Data
            : Scape.History.Generator.Type.PlotTreesDataComponent<NpcPlotTreeManager, Data> { }

          NpcPlotTreeManager(IBuilder builder)
            : base(builder) { }  NpcPlotTreeManager()
            : base() { }

          protected internal override Scape.History ProcessForCurrentTick(Scape.History currentHistory, Scape.Moment.Delta? timeSinceLastTick) {
            _progressActivePlots(currentHistory);
            _refillMissingPotentialPlots();
            _tryToStartPlotsFromPotentialPlots(currentHistory);

            return currentHistory;
          }

          void _progressActivePlots(Scape.History currentHistory) {
            foreach (var activePlot in currentHistory.ActivePlotTrees[PlotTrees.Tags.NpcOwned].Cast<PlotTree.IndividualLevel>()) {
              IEnumerable<PlotTree.IndividualLevel> allActivePlotsForTheCurrentNpc
                = currentHistory.ActivePlotTrees[PlotTrees.Tags.Owner.WithExtraContext(activePlot.Npc.Id)].Cast<PlotTree.IndividualLevel>();

              // npc plots can only progress if they're in one of the two higest urgency levels out of all urgencey levels of all active plots for this npc
              if (allActivePlotsForTheCurrentNpc?.Count() > 1) {
                int greaterUrgencyCount = 0;
                foreach (PlotTree.IndividualLevel activeNpcPlot in allActivePlotsForTheCurrentNpc) {
                  if (activeNpcPlot.Urgency > activePlot.Urgency) {
                    greaterUrgencyCount++;
                    if (greaterUrgencyCount > 1) {
                      break;
                    }
                  }
                }

                if (greaterUrgencyCount < 2) {
                  activePlot.Progress(currentHistory, Generator);
                }
              }
            }
          }

          void _refillMissingPotentialPlots() {
            Data data = Generator.Archetype.GetComponent<Data>();
            foreach (PlotTree.Type plotTreeType in data.DefaultPlotTreeTypes.Concat(data.PlotTreeInitializationOverrides.Keys).Distinct()) {
              if (data.PlotTreeInitializationMultiplierOverrides.TryGetValue(plotTreeType, out var multiplier)) {
                int current = _potentialTreeCounts.TryToGet(plotTreeType);
                int toAdd = multiplier - current;
                _potentialTrees.AddRange(toAdd.Of(plotTreeType).Cast<PlotTree.IndividualLevel.Type>());
                _potentialTreeCounts[plotTreeType] = multiplier;
              }
              else {
                int current = _potentialTreeCounts.TryToGet(plotTreeType);
                if (current == 0) {
                  _potentialTrees.Add(plotTreeType as PlotTree.IndividualLevel.Type);
                  _potentialTreeCounts[plotTreeType] = 1;
                }
              }
            }
          }

          void _tryToStartPlotsFromPotentialPlots(Scape.History currentHistory) {
            Data data = Generator.Archetype.GetComponent<Data>();

            // TODO: limt plots allowed per npc?
            foreach (PlotTree.IndividualLevel.Type npcPlotType in _potentialTrees.SelectMany(t => _potentialTreeCounts[t].Of(t))) {
              var rootNode = npcPlotType.RootNodeType as PlotTree.IndividualLevel.Node;
              if (rootNode.TryToFindValidPotentialNpcsForAssignment(currentHistory, out var potentialNpcs)) {
                if (rootNode.CalculateChanceAsNextChoice(currentHistory) >= currentHistory.Scape.SeedBasedRandomizer.NextDouble()) {
                  if (rootNode.TryToAssignToAValidPotentialNpc(potentialNpcs, currentHistory, out Entity assignedNpc)) {
                    var plotTree = (data.PlotTreeInitializationOverrides.TryGetValue(npcPlotType, out var initializer)
                        ? initializer(npcPlotType, Generator, currentHistory.ActivePlotTrees[PlotTrees.Tags.Type.WithExtraContext(npcPlotType.Id.Key)].Count())
                        : npcPlotType.Make(assignedNpc, Generator, currentHistory)) as PlotTree.IndividualLevel;

                    plotTree.Npc = assignedNpc;
                    currentHistory._activePlotTrees.Add(
                      new[] {
                        PlotTrees.Tags.NpcOwned,
                        PlotTrees.Tags.Owner.WithExtraContext(assignedNpc.Id),
                        PlotTrees.Tags.Type.WithExtraContext(npcPlotType.Id.Key),
                        PlotTrees.Tags.Id.WithExtraContext(plotTree.Id),
                      }.Concat(plotTree.Tags),
                      plotTree
                    );

                    // tick down the count of this type of tree
                    _potentialTreeCounts[npcPlotType] -= 1;
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}