using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SpiritWorlds.Data {

  public partial class Scape {
    public partial class History
      : IModel.IComponent<History>,
        IModel.IComponent.IIsRestrictedTo<Scape>,
        IComponent.IUseDefaultUniverse,
        IReadableComponentStorage,
        IWriteableComponentStorage 
    {
      Dictionary<string, IComponent> IReadableComponentStorage._componentsByBuilderKey {
        get;
      } = new();

      /// <summary>
      /// The parent scape this history is for.
      /// </summary>
      public Scape Scape {
        get;
      }

      /// <summary>
      /// The first moment/time-0 of this scape.
      /// Usually just year 0
      /// </summary>
      public Moment FirstMoment {
        get;
        private set;
      }

      /// <summary>
      /// The current scape time/age/date in the present 
      /// </summary>
      public Moment CurrentMoment {
        get;
        private set;
      }

      /// <summary>
      /// The index of the current era
      /// </summary>
      public int CurrentEraIndex {
        get;
        private set;
      } = 0;

      /// <summary>
      /// How much time has passed within the current era
      /// </summary>
      public Moment.Delta TimePassedInCurrentEra {
        get;
        private set;
      } = new();

      /// <summary>
      /// The Non player characters/entities who are already involved in the history of this scape.
      /// </summary>
      public IReadOnlyTagedCollection<Tag, Entity> TrackedNpcs {
        get;
        private set;
      }

      /// <summary>
      /// The plot trees still adding to the current history, with active nodes.
      /// </summary>
      public IReadOnlyTagedCollection<Tag, PlotTree> ActivePlotTrees {
        get => _activePlotTrees;
      } //TOOD: implement add and remove plot tree functionality via public functions
      internal TagedCollection<Tag, PlotTree> _activePlotTrees
        = new();

      /// <summary>
      /// The eras used to generate this world, in order.
      /// </summary>
      public IReadOnlyList<Era> Eras {
        get => _eras;
        protected set => _eras = value.ToList();
      } List<Era> _eras = new();

      /// <summary>
      /// The current era
      /// </summary>
      public Era CurrentEra
        => Eras[CurrentEraIndex];

      /// <summary>
      /// A log entry for an event in the history of a scape.
      /// </summary>
      public record LogEntry(
        float TimeStamp,
        [NotNull] string Text,
        Tile.Key Location,
        [NotNull] IEnumerable<Entity> Participants,
        [AllowNull] PlotTree.Node PlotNode
      );

      /// <summary>
      /// Try to insera an era before another. returns false if the era's already occured.
      /// </summary>
      public bool TryToInsertEra(Era era, Era before) {
        int indexOfBeforeEra = _eras.IndexOf(before);
        if (indexOfBeforeEra <= CurrentEraIndex) {
          return false;
        }
        _eras.Insert(indexOfBeforeEra, era);
        return true;
      }

      /// <summary>
      /// Try to insera an era after another. returns false if the era would have alredy occured/begun
      /// </summary>
      /// <param name="after">(optional)If provided, apends after the given era. If null this appends to the end.</param>
      public bool TryToAppendEra(Era era, Era after = null) {
        if (after is not null) {
          int indexOfAfterEra = _eras.IndexOf(after);
          if (indexOfAfterEra + 1 <= CurrentEraIndex) {
            return false;
          }
          _eras.Insert(indexOfAfterEra + 1, era);
        } else {
          _eras.Add(era);
        }

        return true;
      }

      /// <summary>
      /// Try to remove an era, returns false if the era's already occured.
      /// </summary>
      public bool TryToRemoveEra(Era era) {
        int eraIndex = _eras.IndexOf(era);
        if (eraIndex <= CurrentEraIndex) {
          return false;
        }

        _eras.RemoveAt(eraIndex);
        return true;
      }

      /// <summary>
      /// Used by the game engine to move history forward uring pre-gen and during gameplay time.
      /// </summary>
      internal void _progressForwardInTime(Scape.Moment.Delta delta) {
        // TODO: have the Era set the tick delta. Have era type determine if it's in the past (quick gen/per game day) vs present (real time gen/per game hour)
        foreach(Generator generator in CurrentEra.Generators) {
          generator.ProcessTick(this, delta);
        }

        CurrentMoment += delta;
        TimePassedInCurrentEra += delta;
        if (CurrentEraIndex < Eras.Count - 1 && TimePassedInCurrentEra >= CurrentEra.Length) {
          CurrentEraIndex++;
          TimePassedInCurrentEra = new();
        }
      }
    }
  }
}
