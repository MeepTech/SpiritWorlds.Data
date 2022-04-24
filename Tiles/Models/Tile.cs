using Meep.Tech.Data;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A tile in the world
  /// </summary>
  public partial struct Tile :
    Model<Tile, Tile.Type>.IFromInterface,
    IModel.IUseDefaultUniverse {

    /// <summary>
    /// All tile types
    /// </summary>
    public static Type.Collection Types  {
      get; 
    } = new();

    /// <summary>
    /// the archetype used to build this tile.
    /// </summary>
    public Type Archetype {
      get;
      private set;
    } Type Model<Tile, Type>.IFromInterface.Archetype {
      get => Archetype;
      set => Archetype = value;
    }
  }
}
