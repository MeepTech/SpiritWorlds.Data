using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public partial struct Stat {
    public partial struct Modifier {

      /// <summary>
      /// Contains the logic to apply stat modifiers.
      /// </summary>
      public class Applier : Enumeration<Applier> {
        GetBonusFunction _getBounus;

        /// <summary>
        /// The name of this modifier (Like "Times Five")
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A short description of this modifier
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// How this modifies the stat from the original. Returns an ammount to add to the base stat value.
        /// DO NOT CALL VALUE ON stat. This will result in a stack overflow. Call 'stat.Base' instead.
        /// </summary>
        /// <param name="stat">The original stat before modification</param>
        /// <param name="currentModifiedValue">The current value as it's being updated by other modifiers</param>
        public delegate float GetBonusFunction(IStat stat, float currentModifiedValue);

        public Applier(string key, string name, GetBonusFunction getBonusFunction, string description = null)
          : base(key) { Name = name; Description = description; _getBounus = getBonusFunction; }

        /// <summary>
        /// Get the bonus value using this modifier's GetBonusFunction logic.
        /// </summary>
        public float GetBonus(IStat stat, float currentModifiedValue)
          => _getBounus.Invoke(stat, currentModifiedValue);
      }
    }
  }
}