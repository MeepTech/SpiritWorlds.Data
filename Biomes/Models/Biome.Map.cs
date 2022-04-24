using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Geometry;
using Meep.Tech.Geometry.Voronoi;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  public partial class Biome {

    /// <summary>
    /// A 2D map of interconnected biomes made from a voronoi diagram.
    /// Used to generate tileboards.
    /// </summary>
    public partial class Map : Model<Biome.Map, Biome.Map.Type> {
      readonly int _biomeCentersCount;
      // TODO: find a good way to serialize and deserialize these.
      readonly Dictionary<string, Biome> _biomes = new();
      readonly Dictionary<Coordinate, Vertex> _circumcenters = new();
      readonly Map<string, Coordinate> _biomeCircumcenters = new();

      /// <summary>
      /// The tileboard this biome map is attached to.
      /// </summary>
      public Tile.Board Board {
        get;
      }

      /// <summary>
      /// Make a new biome map for a new scape
      /// </summary>
      /// <param name="board"></param>
      /// <param name="biomeCentersCount"></param>
      public Map(Tile.Board board, int biomeCentersCount = 100) : this() {
        Board = board;
        _biomeCentersCount = biomeCentersCount;
        _initializeVoronoiDelaunay();
      } Map() { }

      /// <summary>
      /// Get which biome a tile is in.
      /// </summary>
      /// <param name="tile">The board level tile key</param>
      /// <param name="nearbyBiome">(Optional)A biome you know is nearby. This can help narrow the search to the nearby biome and it's neighbors</param>
      /// <returns>The biome at the given tile location</returns>
      public Biome Get(Tile.Key tile, Biome nearbyBiome = null) {
        Coordinate location = tile.GetBoardLocation(Board).AsPoint();
        if (nearbyBiome is not null) {
          return _findClosestCircumcenter(location, _circumcenters[_biomeCircumcenters.Forward[nearbyBiome.Id]].OutgoingVectors.Values.Select(v => {
            var circumcenterLocation = (v.PointsTo.X, v.PointsTo.Y).AsCoordinate();
            return (_biomes[_biomeCircumcenters.Reverse[circumcenterLocation]], _circumcenters[circumcenterLocation]);
          }).Prepend((nearbyBiome, _circumcenters[_biomeCircumcenters.Forward[nearbyBiome.Id]]))).biome;
        } else {
          return _findClosestCircumcenter(location, _biomeCircumcenters.Select(e => (_biomes[e.Forward], _circumcenters[e.Reverse]))).biome;
        }
      }

      (Biome biome, Vertex circumcenter) _findClosestCircumcenter(Coordinate location, IEnumerable<(Biome biome, Vertex circumcenter)> biomesWithCenters) {
        ((Biome biome, Vertex circumcenter)? value, double distance) closest = (null, double.MaxValue);
        foreach(var entry in biomesWithCenters) {
          double distance = entry.circumcenter.AsCoordinate().Distance(location);
          if (distance < closest.distance) {
            closest = (entry, distance);
          }
        }

        return closest.value ?? throw new System.Exception($"Somehow could not find a closest item.");
      }

      void _initializeVoronoiDelaunay() {
        /// Generate the biome map using a voronoi diagram with a random set of points
        List<(float x, float z)> biomeCircumcenters = new();

        //Generate the random sites
        _generateRandomCircumcenterCoordinates(biomeCircumcenters);

        // add a star around the random points to clean them up
        _generateOuterBoundsStar(biomeCircumcenters);

        // generate the cells based on the centers
        Dictionary<Vertex, Polygon> voronoiCells = Delaunay.GenerateVoronoiCells(
          Delaunay.GenerateTriangulation(biomeCircumcenters)
        );

        // generate the biomes from the voronoi cells.
        foreach (Polygon voronoiCell in voronoiCells.Values) {
          _assignBiomeToVoronoiCell(voronoiCell);
        }
      }

      void _generateRandomCircumcenterCoordinates(List<(float x, float z)> biomeCircumcenters) {
        var circumcenterRandomness = new System.Random(Board.Scape.Seed);
        for (int i = 0; i < _biomeCentersCount; i++) {
          int randomX = circumcenterRandomness.Next(0, Board.Dimensions.EastWest);
          int randomZ = circumcenterRandomness.Next(0, Board.Dimensions.NorthSouth);

          biomeCircumcenters.Add((randomX, randomZ));
        }
      }

      void _generateOuterBoundsStar(List<(float x, float z)> biomeCircumcenters) {
        biomeCircumcenters.Add((0f, Board.Dimensions.NorthSouth / 2 * 5));
        biomeCircumcenters.Add((0f, -Board.Dimensions.NorthSouth / 2 * 5));
        biomeCircumcenters.Add((Board.Dimensions.EastWest / 2 * 5, 0f));
        biomeCircumcenters.Add((-Board.Dimensions.EastWest / 2 * 5, 0f));
      }

      void _assignBiomeToVoronoiCell(Polygon voronoiCell) {
        // find out which type of biome should go at this circumcenter
        Biome.Type biomeType = Archetype.GetBiomeType(voronoiCell, Board);
        _circumcenters.Add((voronoiCell.Center.X, voronoiCell.Center.Y).AsCoordinate(), voronoiCell.Center);

        /// check if an existing neighboring biome of the same type should just extend into this one
        voronoiCell.Center.ForEachOutgoingVector((delaunayEdge, _) => {
          if (_biomeCircumcenters.Reverse.TryGetValue((delaunayEdge.PointsTo.X, delaunayEdge.PointsTo.Y).AsCoordinate(), out string neighboringCellBiomeId)
            && _biomes[neighboringCellBiomeId].Archetype == biomeType
          ) {
            // Set the biome to use and end the loop
            _biomeCircumcenters.Add(neighboringCellBiomeId, (voronoiCell.Center.X, voronoiCell.Center.Y).AsCoordinate());
            _biomes[neighboringCellBiomeId]._addPolygon(voronoiCell);
            return;
          }
        });

        // if there's no existing biome, we need to make a new one
        Biome newBiome = biomeType.Make(
          Board,
          voronoiCell,
          Archetype.GetBiomeBaseHeight(biomeType, voronoiCell, Board)
        );
        _biomes.Add(newBiome.Id, newBiome);
        _biomeCircumcenters.Add(newBiome.Id, (voronoiCell.Center.X, voronoiCell.Center.Y).AsCoordinate());
      }
    }
  }
}
