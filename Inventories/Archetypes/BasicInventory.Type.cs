using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace SpiritWorlds.Data {
  public static partial class Inventories {
    public sealed partial class BasicInventory {

      /// <summary>
      /// The type for a basic inventory.
      /// </summary>
      [Branch]
      public new class Type : Inventory.Type {

        /// <summary>
        /// The id for a basic inventory.
        /// </summary>
        public new static Identity Id {
          get;
        } = new Identity(nameof(BasicInventory));

        public override string Description
          => "A basic, unordered inventory used to hold any numer of item stacks";

        Type()
          : base(Id) { }
      }
    }
  }
}
