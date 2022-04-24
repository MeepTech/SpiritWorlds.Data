using Meep.Tech.Data;
using Meep.Tech.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiritWorlds.Data {
  public partial struct Tile {

    /// <summary>
    /// A board of tile stacks.
    /// </summary>
    public partial class Board
      : Model<Board>,
        IUnique,
        IModel.IUseDefaultUniverse,
        IReadOnlyDictionary<Tile.Key, Tile>, 
        IReadOnlyDictionary<Board.Chunk.Key, Board.Chunk>
    {

      /// <summary>
      /// The unique id of this board
      /// </summary>
       public string Id {
        get; 
        private set; 
      } string IUnique.Id {
        get => Id; 
        set => Id = value; 
      }

      /// <summary>
      /// The shape of chunks in this board.
      /// </summary>
      public Chunk.Shape ChunksShape { 
        get;
        set;
      }

      /// <summary>
      /// The scape this tile board is in.
      /// </summary>
      public Scape Scape {
        get;
      }
      
      /// <summary>
      /// The overall size/dimensions of this board, in tiles (X, Y, Z)
      /// </summary>
      public (int EastWest, int Height, int NorthSouth) Dimensions {
        get;
      }

      /// <summary>
      /// The bounds of this board in tiles, this is how far tiles are allowed to be from the origin in each direction.
      /// </summary>
      public ((int West, int Bottom, int South) BottomSouthWest, (int East, int Top, int North) TopNorthEast) Bounds {
        get;
      }

      /// <summary>
      /// The offset of the tile location of the center/origin/(0,0,0) point of this board if you consider the BottomSouthWest bound as the actual 0,0,0
      /// </summary>
      public Point Origin {
        get;
      }

      /// <summary>
      /// Can be used to access noise layers cached for generating the current overall scape
      /// </summary>
      public NoiseLayersManager NoiseLayers {
        get;
        private set;
      }

      /// <summary>
      /// Make a new scape
      /// </summary>
      /// <param name="seed">The seed of the new scape</param>
      /// <param name="maxDimensionsInTiles">How big in tiles the scape can be</param>
      /// <param name="originOffset">(optional) a vector to offset the origin by. By default, the origin is put at the very center of the scape.</param>
      public Board(Scape scape, (int eastWest, int height, int northSouth) maxDimensionsInTiles, Point? originOffset = null)
        : this() {
        Scape = scape;
        NoiseLayers = new NoiseLayersManager(Scape.Seed);
        Dimensions = maxDimensionsInTiles;
        Origin = (maxDimensionsInTiles.AsPoint() / 2) + (originOffset ?? Point.Zero);
        Bounds = _calculateBounds(Dimensions, Origin);
      } Board() { }

      static (Point bottomSouthWest, Point topNorthEast) _calculateBounds((int eastWest, int height, int northSouth) dimensions, Point origin)
        => (origin - dimensions.AsPoint() / 2, origin + dimensions.AsPoint() / 2);

      #region IReadOnlyDictionary[Tiles]

      Tile IReadOnlyDictionary<Tile.Key, Tile>.this[Tile.Key key]
        => _chunks[key.ChunkKey.Value][key];

      IEnumerable<Tile.Key> IReadOnlyDictionary<Tile.Key, Tile>.Keys
        => _chunks.SelectMany(e => e.Value.Select(ee => new Tile.Key {
          ChunkKey = e.Key,
          X = ee.Key.X,
          Z = ee.Key.Z,
          Height = ee.Key.Height,
        }));

      IEnumerable<Tile> IReadOnlyDictionary<Tile.Key, Tile>.Values
        => _chunks.SelectMany(e => e.Value.Values);

      bool IReadOnlyDictionary<Tile.Key, Tile>.ContainsKey(Tile.Key key) {
        if (key.ChunkKey.HasValue) {
          return _chunks.TryGetValue(key.ChunkKey.Value, out var chunk)
            ? chunk.ContainsKey(key)
            : false;
        }

        return false;
      }

      IEnumerator<KeyValuePair<Tile.Key, Tile>> IEnumerable<KeyValuePair<Tile.Key, Tile>>.GetEnumerator()
        => _chunks.SelectMany(e => e.Value.Select(ee => new KeyValuePair<Tile.Key, Tile>(ee.Key, ee.Value))).GetEnumerator();

      bool IReadOnlyDictionary<Tile.Key, Tile>.TryGetValue(Tile.Key key, out Tile value) {
        if (key.ChunkKey.HasValue)
          if (_chunks.TryGetValue(key.ChunkKey.Value, out var chunk))
            if (chunk.TryGetValue(key, out value))
              return true;

        value = default;
        return false;
      }

      #endregion

      #region IReadOnlyDictionary[Chunks]

      Dictionary<Board.Chunk.Key, Board.Chunk> _chunks
        = new();

      public Chunk this[Chunk.Key key]
        => ((IReadOnlyDictionary<Chunk.Key, Chunk>)_chunks)[key];

      public IEnumerable<Chunk.Key> Keys
        => ((IReadOnlyDictionary<Chunk.Key, Chunk>)_chunks).Keys;

      public IEnumerable<Chunk> Values
        => ((IReadOnlyDictionary<Chunk.Key, Chunk>)_chunks).Values;

      public int Count
        => ((IReadOnlyCollection<KeyValuePair<Chunk.Key, Chunk>>)_chunks).Count;

      public bool ContainsKey(Chunk.Key key) {
        return ((IReadOnlyDictionary<Chunk.Key, Chunk>)_chunks).ContainsKey(key);
      }

      public IEnumerator<KeyValuePair<Chunk.Key, Chunk>> GetEnumerator() {
        return ((IEnumerable<KeyValuePair<Chunk.Key, Chunk>>)_chunks).GetEnumerator();
      }

      public bool TryGetValue(Chunk.Key key, out Chunk value) {
        return ((IReadOnlyDictionary<Chunk.Key, Chunk>)_chunks).TryGetValue(key, out value);
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)_chunks).GetEnumerator();
      }

#endregion
    }
  }
}
