namespace SpiritWorlds.Data {
  public static partial class Stats {

    /// <summary>
    /// The base type for status effect resistances
    /// </summary>
    public abstract class StatusEffectResistance : Stat.Type {
      public StatusEffectResistance(
         string keyPrefix,
         string name,
         string acronym,
         string description,
         int? upperBar = 25,
         int? lowerBar = null,
         int? defaultValue = null
        ) : base(
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
