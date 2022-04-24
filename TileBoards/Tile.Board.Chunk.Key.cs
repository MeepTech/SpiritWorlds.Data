using System;

namespace SpiritWorlds.Data {
  public partial struct Tile {
    public partial class Board {
      public partial class Chunk {

        /// <summary>
        /// The key to find a chunk in a board.
        /// </summary>
        public struct Key {
          public Board.Key BoardKey { get; }
          public int X { get; init; }
          public int Z { get; init; }
          public override int GetHashCode()
            => HashCode.Combine(X, Z);
        }
      }
    }
  }
}
