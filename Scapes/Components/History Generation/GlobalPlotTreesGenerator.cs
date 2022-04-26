using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {
  public partial class Scapes {
    public partial class Histories {
      public partial class Generators {

        /// <summary>
        /// A component that can be used to dictate how global plots are activated and moved forward between ticks.
        /// </summary>
        public class GlobalPlotTreesGenerator
          : Scapes.Histories.Generators.PlotTreeHistoryGenerator.Type
        {

          public new static Identity Id {
            get;
          } = new Identity("Base Global Plot Tree Based History Generator");

          protected GlobalPlotTreesGenerator(Identity id) 
            : base(id ?? Id) {}

          protected internal override Scape.History ProcessTick(Scape.History.Generator generator, Scape.History currentHistory, Scape.Moment.Delta? tickLength) {
            _progressActivePlots(currentHistory, generator);
            _refillMissingPotentialPlots(generator as PlotTreeHistoryGenerator);
            _tryToStartPlotsFromPotentialPlots(currentHistory, generator as PlotTreeHistoryGenerator);

            return currentHistory;
          }

          void _progressActivePlots(Scape.History currentHistory, Scape.History.Generator generator) {
            foreach (var activePlot in currentHistory.ActivePlotTrees[PlotTrees.Tags.GlobalyOwned]) {
              if (activePlot.ActiveNode.CalculateProgressionChance(currentHistory, generator) >= currentHistory.Scape.SeedBasedRandomizer.NextDouble()) {
                activePlot.Progress(currentHistory, generator);
              }
            }
          }

          void _refillMissingPotentialPlots(PlotTreeHistoryGenerator generator) {
            foreach (PlotTree.Type plotTreeType in DefaultPlotTreeTypes.Concat(PlotTreeInitializationOverrides.Keys).Distinct()) {
              if (PlotTreeInitializationMultiplierOverrides.TryGetValue(plotTreeType, out var multiplier)) {
                int current = GetPotentialTreeCountsFrom(generator).TryToGet(plotTreeType);
                int toAdd = multiplier - current;
                GetPotentialTreesFrom(generator).AddRange(toAdd.Of(plotTreeType).Cast<PlotTree.GlobalLevel.Type>());
                GetPotentialTreeCountsFrom(generator)[plotTreeType] = multiplier;
              }
              else {
                int current = GetPotentialTreeCountsFrom(generator).TryToGet(plotTreeType);
                if (current == 0) {
                  GetPotentialTreesFrom(generator).Add(plotTreeType as PlotTree.GlobalLevel.Type);
                  GetPotentialTreeCountsFrom(generator)[plotTreeType] = 1;
                }
              }
            }
          }

          void _tryToStartPlotsFromPotentialPlots(Scape.History currentHistory, PlotTreeHistoryGenerator generator) {
            foreach (PlotTree.GlobalLevel.Type globalPlotTreeType in GetPotentialTreesFrom(generator).SelectMany(t => GetPotentialTreeCountsFrom(generator)[t].Of(t))) {
              if (globalPlotTreeType.RootNodeType.CalculateChanceAsNextChoice(currentHistory) >= currentHistory.Scape.SeedBasedRandomizer.NextDouble()) {
                // build the new tree and add it to the active plots 
                var plotTree = PlotTreeInitializationOverrides.TryGetValue(globalPlotTreeType, out var initializer)
                  ? initializer(globalPlotTreeType, generator, currentHistory.ActivePlotTrees[PlotTrees.Tags.Type.WithExtraContext(globalPlotTreeType.Id.Key)].Count())
                  : globalPlotTreeType.Make(generator, currentHistory);

                currentHistory._activePlotTrees.Add(
                  new[] {
                    PlotTrees.Tags.GlobalyOwned,
                    PlotTrees.Tags.Type.WithExtraContext(globalPlotTreeType.Id.Key),
                    PlotTrees.Tags.Id.WithExtraContext(plotTree.Id),
                  }.Concat(plotTree.Tags),
                  plotTree
                );

                // tick down the count of this type of tree
                GetPotentialTreeCountsFrom(generator).[globalPlotTreeType] -= 1;
              }
            }
          }
        }
      }
    }
  }
}