namespace SpiritWorlds.Data {
  public static partial class Stats {

    /// <summary>
    /// Base type for a damage multiplier stat
    /// </summary>
    public abstract class DamageMultiplier : Stat.Type {
      protected DamageMultiplier(
        string keyPrefix,
        string name,
        string acronym,
        string description,
        int? upperBar = 25,
        int? lowerBar = null,
        int? defaultValue = null)
      : base(
        // TODO: add damage type enum.
        keyPrefix,
        name,
        acronym,
        description,
        upperBar,
        lowerBar,
        defaultValue
      ) { }
    }
  }
}
