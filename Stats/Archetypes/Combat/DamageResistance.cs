namespace SpiritWorlds.Data {
  public static partial class Stats {

    /// <summary>
    /// The base class for stats that resist damage
    /// </summary>
    public abstract class DamageResistance : Stat.Type {
      protected DamageResistance(
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
