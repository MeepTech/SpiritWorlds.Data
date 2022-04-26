using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public partial class Scape {
    public partial class History {
      public partial class Generator {
        public abstract partial class Type : Archetype<Generator, Generator.Type> {
          protected Type(Archetype.Identity id) 
            : base(id) {}

          internal protected abstract History ProcessTick(
            Generator generator,
            History currentHistory, 
            Moment.Delta? tickLength
          );
        }
      }
    }
  }
}
