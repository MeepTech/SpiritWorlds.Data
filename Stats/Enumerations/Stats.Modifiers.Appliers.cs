namespace SpiritWorlds.Data {
  public static partial class Stats {
    public static partial class Modifiers {

      // TODO: add a lua logic applier one day.
      public class Appliers : Stat.Modifier.Applier {

        /// <summary>
        /// A basic plus one modifier
        /// </summary>
        public static Appliers Plus1 {
          get;
        } = new Appliers(
          nameof(Plus1),
          "Plus One",
          (_, _) => 1,
          "Add one to the base stat value"
        );

        internal Appliers(string keyEnding, string name, GetBonusFunction getBonusFunction, string description = null)
          : base($"Included.{keyEnding}", name, getBonusFunction, description) { }

        /// <summary>
        /// Make a special applier that takes a variable and adds the value.
        /// </summary>
        public static Appliers Plus(float x)
          => new(
            "Plus(X)",
            "Plus X",
            (_, _) => x,
            "Add X to the base stat value"
          );
      }
    }
  }
}