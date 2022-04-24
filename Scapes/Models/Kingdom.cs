using Meep.Tech.Data;

namespace SpiritWorlds.Data {

  /// <summary>
  /// The Base Model for all Kingdoms
  /// </summary>
  public class Kingdom : Model<Kingdom, Kingdom.Type>, IModel.IUseDefaultUniverse {

    /// <summary>
    /// The Base Archetype for Kingdoms
    /// </summary>
    public abstract class Type : Archetype<Kingdom, Kingdom.Type> {

      /// <summary>
      /// Used to make new Child Archetypes for Kingdom.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }
    }
  }

  public static class Kingdoms {

    /// <summary>
    /// The general type of kingdom, a randomly generated one.
    /// </summary>
    public class RandomlyGenerated : Kingdom.Type {

      RandomlyGenerated() 
        : base(new Identity("Random", "Basic")) {}
    }
  }
}
