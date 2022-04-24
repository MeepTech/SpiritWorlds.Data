using Meep.Tech.Data;
using Meep.Tech.Geometry;

namespace SpiritWorlds.Data {

  public partial class Biome {
    public partial class Map {
      public abstract class Type : Archetype<Biome.Map, Biome.Map.Type> {
        public Type(Archetype.Identity id) 
          : base(id) {}

        /// <summary>
        /// Get the type of biome that the given polygon should contain for a given scape.
        /// </summary>
        public abstract Biome.Type GetBiomeType(Polygon biomeVoronoiPolygon, Tile.Board board);

        /// <summary>
        /// Get the average/base/circumcenter height for a biome cell.
        /// </summary>
        public abstract int GetBiomeBaseHeight(Biome.Type biomeType, Polygon polygon, Tile.Board board);
          // return board.Scape.NoiseLayers[Scapes.NoiseLayers.HeightMap].GetPerlin(polygon.Center.X, polygon.Center.Y);
      }
    }
  }
}
