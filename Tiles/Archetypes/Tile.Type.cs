using Meep.Tech.Data;

namespace SpiritWorlds.Data {

  public partial struct Tile {
    /// <summary>
    /// An archetype used to make new tiles
    /// </summary>
    public abstract class Type : Archetype<Tile, Tile.Type> {

      /// <summary>
      /// Constructor used to maken new tile types to be loaded by xbam
      /// </summary>
      /// <param name="id"></param>
      protected Type(Archetype.Identity id) 
        : base(id, Types) {}

      /// <summary>
      /// Make a new single tile of this type.
      /// </summary>
      public new Tile Make()
        => base.Make();

      /// <summary>
      /// Make a new single tile of this type.
      /// </summary>
      public Tile.Column MakeColumn(int? height = 1, int? baseHeightLocation = 0)
        => new(base.Make(), height, baseHeightLocation);
    }
  }
}
