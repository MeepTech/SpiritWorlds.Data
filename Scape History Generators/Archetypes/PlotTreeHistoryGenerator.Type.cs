using Meep.Tech.Data;
using System.Collections.Generic;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace SpiritWorlds.Data {
  public static partial class Scapes {
    public partial class Histories {
      public partial class Generators {
        public partial class PlotTreeHistoryGenerator {
          internal List<PlotTree.Type> _potentialTrees { get; private set; } = new();
          internal Dictionary<PlotTree.Type, int> _potentialTreeCounts { get; private set; } = new();

          /// <summary>
          /// Used to hold data for different kinds of plot trees that can be added during world history gen.
          /// </summary>
          [Branch]
          public new abstract class Type : Scape.History.Generator.Type {

            /// <summary>
            /// An initializer for a plot tree
            /// </summary>
            /// <param name="typeToBuild">The type of plot tree being made</param>
            /// <param name="generator">The generator building the plot trees</param>
            /// <param name="index">the index of this plot tree of this kind that was produced this same tic. (0 if this is the first/only plot tree of this kind produced this tic)</param>
            /// <returns></returns>
            public delegate PlotTree PlotTreeInitializer(PlotTree.Type typeToBuild, Scape.History.Generator generator, int index);

            #region Overrideable Archetype Properties

            /// <summary>
            /// Plot trees added to the generated generator type on init.
            /// You can use PlotTreeInitializationOverrides to override the default initializer per type, 
            ///   as well as PlotTreePerGeneratorLimits and PlotTreeInitializationMultiplierOverrides to modify how numerous/common the plot tree can be in the world generation.
            /// </summary>
            public virtual IEnumerable<PlotTree.Type> DefaultPlotTreeTypes {get => _DefaultPlotTreeTypes ?? new List<PlotTree.Type>();}
            /** <summary> The backing field used to initialize and override the initail value of DefaultPlotTreeTypes. You can this syntax to override or add to the base initial value: '=> _DefaultPlotTreeTypes ??= base.DefaultPlotTreeTypes.Append(...' </summary> **/
            protected IEnumerable<PlotTree.Type> _DefaultPlotTreeTypes { get => _defaultPlotTreeTypes; set => _defaultPlotTreeTypes = value; }
            IEnumerable<PlotTree.Type> _defaultPlotTreeTypes;

            /// <summary>
            /// Plot tree types that are initialized with multiple individual trees allowed at a time.
            /// Differs from PlotTreePerGeneratorLimits in that this is how many of this tree are allowed to be assigned at one time, while PlotTreePerGeneratorLimits is how many are allowed overall across all history.
            /// </summary>
            public virtual IReadOnlyDictionary<PlotTree.Type, int> PlotTreeInitializationMultiplierOverrides {
              get => _PlotTreeInitializationMultiplierOverrides ?? new();
            } /** <summary> The backing field used to initialize and override the initail value of PlotTreeInitializationMultiplierOverrides. 
              * You can this syntax to override or add to the base initial value:
              *   '=> _PlotTreeInitializationMultiplierOverrides ??= base.PlotTreeInitializationMultiplierOverrides.Append(...' </summary> 
              **/
            protected Dictionary<PlotTree.Type, int> _PlotTreeInitializationMultiplierOverrides {
              get => _plotTreeInitializationMultiplierOverrides; set => _plotTreeInitializationMultiplierOverrides = value;
            } Dictionary<PlotTree.Type, int> _plotTreeInitializationMultiplierOverrides;

            /// <summary>
            /// Plot tree types that are limited to a certain numer of instances.
            /// Null, or missing types default to infinite, but one potential at a time.
            /// Differs from PlotTreeInitializationMultiplierOverrides in that this is how many of this tree are allowed overall across all history, while PlotTreeInitializationMultiplierOverrides is how many are allowed to be assigned at any one given time.
            /// </summary>
            public virtual IReadOnlyDictionary<PlotTree.Type, int> PlotTreePerGeneratorLimits {
              get => _PlotTreePerGeneratorLimits ?? new();
            } /** <summary> The backing field used to initialize and override the initail value of PlotTreePerGeneratorLimits. 
              * You can this syntax to override or add to the base initial value:
              *   '=> _PlotTreePerGeneratorLimits ??= base.PlotTreePerGeneratorLimits.Append(...' </summary> 
              **/
            protected Dictionary<PlotTree.Type, int> _PlotTreePerGeneratorLimits {
              get => _plotTreePerGeneratorLimits; set => _plotTreePerGeneratorLimits = value;
            } Dictionary<PlotTree.Type, int> _plotTreePerGeneratorLimits;

            /// <summary>
            /// Overrideable initializers for plot trees of certain types.
            /// TODO: what does this do that you can't do via overriding Make for the plottree already?
            /// </summary>
            public virtual IReadOnlyDictionary<PlotTree.Type, PlotTreeInitializer> PlotTreeInitializationOverrides {
              get => _PlotTreeInitializationOverrides ?? new();
            } /** <summary> The backing field used to initialize and override the initail value of PlotTreeInitializationMultiplierOverrides. 
              * You can this syntax to override or add to the base initial value:
              *   '=> _PlotTreeInitializationMultiplierOverrides ??= base.PlotTreeInitializationMultiplierOverrides.Append(...' </summary> 
              **/
            protected Dictionary<PlotTree.Type, PlotTreeInitializer> _PlotTreeInitializationOverrides {
              get => _plotTreeInitializationOverrides; set => _plotTreeInitializationOverrides = value;
            } Dictionary<PlotTree.Type, PlotTreeInitializer> _plotTreeInitializationOverrides;

            #endregion

            protected Type(Archetype.Identity id) 
              : base(id) { }

            protected static List<PlotTree.Type> GetPotentialTreesFrom(PlotTreeHistoryGenerator generator)
              => generator._potentialTrees;

            protected static Dictionary<PlotTree.Type, int> GetPotentialTreeCountsFrom(PlotTreeHistoryGenerator generator)
              => generator._potentialTreeCounts;
          }
        }
      }
    }
  }
}
