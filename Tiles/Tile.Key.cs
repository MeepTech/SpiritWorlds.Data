using Meep.Tech.Geometry;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SpiritWorlds.Data {
  public partial struct Tile {

    /// <summary>
    /// The key to find a tile.
    /// </summary>
    public struct Key {

      /// <summary>
      /// Which level of granularity is assumed by this key's X and Y values
      /// </summary>
      public enum Level {
        Chunk,
        Board
      }

      public int X { get; init; }

      public int? Height { get; init; }

      public int Z { get; init; }

      /// <summary>
      /// The granularity of this tile key.
      /// This tells you if the XYZ is for the whole scape or just the local chunk.
      /// </summary>
      public Level Granularity { get; init; }

      /// <summary>
      /// The chunk key related to this tile key
      /// </summary>
      public Board.Chunk.Key? ChunkKey { 
        get => _chunkKey ??= (Granularity == Level.Chunk ? null : new Board.Chunk.Key() {
          X = X / Tile.Board.Chunk.DefaultDiameter,
          Z = Z / Tile.Board.Chunk.DefaultDiameter
        }); 
        init => _chunkKey = value; 
      } Board.Chunk.Key? _chunkKey;

      /// <summary>
      /// The chunk level location, local to the chunk this tile is in.
      /// This will throw if all the required data is not available.
      /// Scape is only required if the granularity level is scape.
      /// </summary>
      public (int X, int Height, int Z) GetLocalChunkLocation(Tile.Board board = null)
        => Granularity == Level.Chunk
          ? (X, Height ?? 0, Z)
          : ((X + board.Origin.X) % Tile.Board.Chunk.DefaultDiameter, Height ?? 0, (Z + board.Origin.Z) % Tile.Board.Chunk.DefaultDiameter).AsPoint();

      /// <summary>
      /// The global board location.
      /// This will throw if all the required data is not available.
      /// chunkKey is only required if the granilarity level is Chunk and there is no pre-stored chunk key in this key
      /// </summary>
      public (int X, int Height, int Z) GetBoardLocation([NotNull] Tile.Board board, Tile.Board.Chunk.Key? chunkKey = null) {
        if (Granularity == Level.Board) {
          return (X, Height ?? 0, Z);
        }

        Tile.Board.Chunk.Key chunkKeyToUse
          = chunkKey 
            ?? ChunkKey 
            ?? throw new MissingFieldException($"Cannot get a scape location at Chunk granularity from a Tile.Key without a Chunk.Key provided as well.");

        return (chunkKeyToUse.X * Tile.Board.Chunk.DefaultDiameter + X, Height ?? 0, chunkKeyToUse.Z * Tile.Board.Chunk.DefaultDiameter + Z).AsPoint() 
          - board.Origin;
      }

      public override int GetHashCode()
        => HashCode.Combine(X, Z);
    }
  }
}
