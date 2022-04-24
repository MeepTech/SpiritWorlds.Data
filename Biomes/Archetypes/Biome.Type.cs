using Meep.Tech.Data;
using Meep.Tech.Geometry;

namespace SpiritWorlds.Data {

  public partial class Biome {
    /// <summary>
    /// The Base Archetype for Biomes
    /// </summary>
    public abstract class Type : Archetype<Biome, Biome.Type> {

      /// <summary>
      /// Used to make new Child Archetypes for Biome.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }

      /// <summary>
      /// Make a new biome of this type for the given scape.
      /// </summary>
      public Biome Make(Tile.Board board, Polygon voronoiCell, int centerHeight)
        => Make<Biome>((nameof(Tile.Board), board), (nameof(Polygon), voronoiCell), (nameof(BaseHeight), centerHeight));

      /// <summary>
      /// overridable logic called to initialize a biome to a vornoi cell.
      /// </summary>
      protected internal void InitializeBiomeForCell(Biome biome, Polygon polygon) { }

      /// <summary>
      /// Used to generate tile stacks for a given location key within this biome type.
      /// </summary>
      protected internal abstract Tile.Column.Stack GenerateTileStack(Tile.Key tileLocationKey, Biome currentBiome);
    }
  }
}
