using Meep.Tech.Geometry;
using System;
using System.Collections.Generic;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A generated world in SpiritWorlds
  /// </summary>
  public partial class Scape {

    /// <summary>
    /// A date and Time within a world within a scape.
    /// </summary>
    public struct Moment {
      public double _valueInYears;

      public override bool Equals(object obj)
        => obj is Moment otherM && otherM == this;
      public static bool operator ==(Moment a, Moment b)
        => a._valueInYears == b._valueInYears;
      public static bool operator !=(Moment a, Moment b)
        => a._valueInYears != b._valueInYears;
    }

    /// <summary>
    /// An address to narrow down a location in a scape.
    /// TODO: this should be easy to searialize to keys
    /// </summary>
    public struct Location {

      /// <summary>
      /// The scape this location is for.
      /// </summary>
      public Scape Scape;

      /// <summary>
      /// Which board this location is for.
      /// Defaults to the default board(0)
      /// </summary>
      public Tile.Board Board;

      /// <summary>
      /// The settlement this location is in
      /// TODO: kingdom class containing it's polygons and loose tiles
      /// </summary>
       public Kingdom Kingdom;

      /// <summary>
      /// The region of the scape this location is in.
      /// </summary>
      public Biome Biome;

      /// <summary>
      /// The settlement this location is in
      /// TODO: Settlement class containing it's polygons and loose tiles
      /// </summary>
      //public Settlement Settlement;

      /// <summary>
      /// The board voronoi polygon this location is within.
      /// </summary>
      public Polygon Polygon;

      /// <summary>
      /// The Structure this location is in
      /// TODO: Structure class containing it's polygons and loose tiles
      /// </summary>
      //public Structure Structure;

      /// <summary>
      /// The specific tile of the item
      /// </summary>
      public Tile.Key? Tile;
    }

    /// <summary>
    /// The seed of this scape
    /// </summary>
    public int Seed {
      get;
      private set;
    }

    /// <summary>
    /// The seed of this scape
    /// </summary>
    public Random SeedBasedRandomizer 
      => _seedBasedRandomizer ??= new Random(Seed);
    Random _seedBasedRandomizer;

    /// <summary>
    /// Make a new scape
    /// </summary>
    /// <param name="seed">The seed of the new scape</param>
    public Scape(int seed)
      : this() {
      Seed = seed;
    } Scape() { }

    /// <summary>
    /// Calculate/collect/find all Non player characters/entities in this scape who could potentially be involved in history and plot events (but aren't yet).
    /// </summary>
    internal IEnumerable<Entity> _getPotentialNpcs() {
      throw new NotImplementedException();
    }
  }
}
