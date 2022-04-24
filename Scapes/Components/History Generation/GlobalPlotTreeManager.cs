using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data.Components {
  public partial class Scapes {
    public partial class Histories {
      public partial class Generators {
        /// <summary>
        /// A component that can be used to dictate how global plots are activated and moved forward between ticks.
        /// </summary>
        public class GlobalPlotTreeManager
          : Scape.History.Generator.PerTickManagementComponent<GlobalPlotTreeManager> 
        {
          List<PlotTree.GlobalLevel.Type> _potentialTrees = new();
          Dictionary<PlotTree.Type, int> _potentialTreeCounts = new();

          public class Data
            : Scape.History.Generator.Type.PlotTreesDataComponent<GlobalPlotTreeManager, Data> { }

          GlobalPlotTreeManager(IBuilder builder) 
            : base(builder) { } GlobalPlotTreeManager() 
            : base() { }

          protected internal override Scape.History ProcessForCurrentTick(Scape.History currentHistory) {
            _progressActivePlots(currentHistory);
            _refillMissingPotentialPlots();
            _tryToStartPlotsFromPotentialPlots(currentHistory);

            return currentHistory;
          }

          void _progressActivePlots(Scape.History currentHistory) {
            foreach (var activePlot in currentHistory.ActivePlotTrees[PlotTrees.Tags.GlobalyOwned]) {
              if (activePlot.ActiveNode.CalculateProgressionChance(currentHistory, Generator) >= currentHistory.Scape.SeedBasedRandomizer.NextDouble()) {
                activePlot.Progress(currentHistory, Generator);
              }
            }
          }

          void _refillMissingPotentialPlots() {
            Data data = Generator.Archetype.GetComponent<Data>();

            foreach (PlotTree.Type plotTreeType in data.DefaultPlotTreeTypes.Concat(data.PlotTreeInitializationOverrides.Keys).Distinct()) {
              if (data.PlotTreeInitializationMultiplierOverrides.TryGetValue(plotTreeType, out var multiplier)) {
                int current = _potentialTreeCounts.TryToGet(plotTreeType);
                int toAdd = multiplier - current;
                _potentialTrees.AddRange(toAdd.Of(plotTreeType).Cast<PlotTree.GlobalLevel.Type>());
                _potentialTreeCounts[plotTreeType] = multiplier;
              }
              else {
                int current = _potentialTreeCounts.TryToGet(plotTreeType);
                if (current == 0) {
                  _potentialTrees.Add(plotTreeType as PlotTree.GlobalLevel.Type);
                  _potentialTreeCounts[plotTreeType] = 1;
                }
              }
            }
          }

          void _tryToStartPlotsFromPotentialPlots(Scape.History currentHistory) {
            Data data = Generator.Archetype.GetComponent<Data>();

            foreach (PlotTree.GlobalLevel.Type globalPlotTreeType in _potentialTrees.SelectMany(t => _potentialTreeCounts[t].Of(t))) {
              if (globalPlotTreeType.RootNodeType.CalculateChanceAsNextChoice(currentHistory) >= currentHistory.Scape.SeedBasedRandomizer.NextDouble()) {
                // build the new tree and add it to the active plots 
                var plotTree = data.PlotTreeInitializationOverrides.TryGetValue(globalPlotTreeType, out var initializer)
                  ? initializer(globalPlotTreeType, Generator, currentHistory.ActivePlotTrees[PlotTrees.Tags.Type.WithExtraContext(globalPlotTreeType.Id.Key)].Count())
                  : globalPlotTreeType.Make(Generator, currentHistory);

                currentHistory._activePlotTrees.Add(
                  new[] {
                    PlotTrees.Tags.GlobalyOwned,
                    PlotTrees.Tags.Type.WithExtraContext(globalPlotTreeType.Id.Key),
                    PlotTrees.Tags.Id.WithExtraContext(plotTree.Id),
                  }.Concat(plotTree.Tags),
                  plotTree
                );

                // tick down the count of this type of tree
                _potentialTreeCounts[globalPlotTreeType] -= 1;
              }
            }
          }
        }
      }
    }
  }
}