using Meep.Tech.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpiritWorlds.Data {

  public partial class Scape {
    public partial class History {

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
        get => _currentMoment;
      } internal Moment _currentMoment;

      /// <summary>
      /// The Non player characters/entities who are already involved in the history of this scape.
      /// </summary>
      public IEnumerable<Entity> Npcs {
        get;
        private set;
      }

      /// <summary>
      /// The plot trees still adding to the current history, with active nodes.
      /// </summary>
      public IReadOnlyTagedCollection<Tag, PlotTree> ActivePlotTrees {
        get => _activePlotTrees;
      } //TOOD: implement add and remove plot tree functionality via public functions
      internal TagedCollection<Tag, PlotTree> _activePlotTrees;

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
    }
  }
}
