using System;

namespace SpiritWorlds.Data {

  public partial class PlotTree {
    /// <summary>
    /// A plot owned by an invididual.
    /// </summary>
    public partial class IndividualLevel : PlotTree {
      private Entity npc;

      /// <summary>
      /// How urgent the plot tree is
      /// </summary>
      public enum UrgencyTypes {
        Novel,
        Casual,
        Nescisary,
        Important,
        Urgent
      }

      /// <summary>
      /// How urgent this plot is to the npc
      /// </summary>
      public UrgencyTypes Urgency {
        get;
      }

      /// <summary>
      /// The npc this plot is linked to/owned by
      /// </summary>
      public Entity Npc {
        get => npc;
        internal set {
          npc = value;
          OnNpcSet();
        }
      }

      /// <summary>
      /// Optional override
      /// Is called on the npc being set.
      /// </summary>
      protected virtual void OnNpcSet() { }
    }
  }
}
