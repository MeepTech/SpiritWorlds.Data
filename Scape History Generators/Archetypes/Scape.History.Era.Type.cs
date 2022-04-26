using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public partial class Scape {
    public partial class History {

      public partial class Era {
        public class Type : Archetype<Era, Era.Type> {
          protected Type(Identity id)
            : base(id) { }
        }
      }
    }
  }
}
