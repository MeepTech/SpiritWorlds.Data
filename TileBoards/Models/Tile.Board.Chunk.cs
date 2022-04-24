using System.Collections;
using System.Collections.Generic;

namespace SpiritWorlds.Data {
  public partial struct Tile {
    public partial class Board {
      public partial class Chunk : IReadOnlyDictionary<Tile.Key, Tile> {

        /// <summary>
        /// The default diameter of a chunk in tiles.
        /// </summary>
        public const int DefaultDiameter = 64;

        /// <summary>
        /// Potential shapes for a chunk
        /// </summary>
        public enum Shape {
          Rectangular,
          Hexagonal
        }

        #region IReadonlyDictionary

        Dictionary<Tile.Key, Tile> _tiles
          = new();

        public Tile this[Tile.Key key]
          => ((IReadOnlyDictionary<Tile.Key, Tile>)_tiles)[key];

        public IEnumerable<Tile.Key> Keys
          => ((IReadOnlyDictionary<Tile.Key, Tile>)_tiles).Keys;

        public IEnumerable<Tile> Values
          => ((IReadOnlyDictionary<Tile.Key, Tile>)_tiles).Values;

        public int Count
          => ((IReadOnlyCollection<KeyValuePair<Tile.Key, Tile>>)_tiles).Count;

        public bool ContainsKey(Tile.Key key) {
          return ((IReadOnlyDictionary<Tile.Key, Tile>)_tiles).ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<Tile.Key, Tile>> GetEnumerator() {
          return ((IEnumerable<KeyValuePair<Tile.Key, Tile>>)_tiles).GetEnumerator();
        }

        public bool TryGetValue(Tile.Key key, out Tile value) {
          return ((IReadOnlyDictionary<Tile.Key, Tile>)_tiles).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
          return ((IEnumerable)_tiles).GetEnumerator();
        }

        #endregion
      }
    }
  }
}
