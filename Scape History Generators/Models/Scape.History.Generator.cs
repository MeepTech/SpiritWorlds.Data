using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public partial class Scape {
    public partial class History {

      /// <summary>
      /// Used to generate history. Part of an era that does something each tick.
      /// </summary>
      public partial class Generator : Model<Generator, Generator.Type>, IModel.IUseDefaultUniverse {

        /// <summary>
        /// Process the history for the current tick of world time.
        /// <paramref name="tickLength">The change in time since the last tick, or null if this is the first tick</paramref>
        /// </summary>
        internal virtual protected History ProcessTick(History currentHistory, Scape.Moment.Delta? tickLength = null)
          => Archetype.ProcessTick(this, currentHistory, tickLength);
      }
    }
  }
}
