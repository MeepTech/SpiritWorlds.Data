using System;

namespace SpiritWorlds.Data {
  public partial struct Tile {

public partial class Board {

      /// <summary>
      /// The key to find a board within a scape.
      /// </summary>
      public struct Key {
        public string Id { get; init; }
        public override int GetHashCode() 
          => HashCode.Combine(Id);
      }
    }
  }
}
