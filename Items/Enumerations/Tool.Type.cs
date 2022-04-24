using Meep.Tech.Data;

namespace SpiritWorlds.Data.Components {
  public static partial class Items {
    public partial class Tool {

      /// <summary>
      /// Tool Types
      /// </summary>
      public class Type : Enumeration<Type> {
        public Type(string uniqueName, Universe universe = null)
          : base(uniqueName) { }
      }
    }
  }
}
