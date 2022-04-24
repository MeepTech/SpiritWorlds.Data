using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A generated biome in a scape
  /// </summary>
  public partial class Biome
    : Model<Biome, Biome.Type>.WithComponents,
      ICached<Biome>,
      IModel.IUseDefaultUniverse {
    readonly List<Polygon> _polygons;

    /// <summary>
    /// The Id of this biome
    /// </summary>
    public string Id {
      get => Id;
    }
    string IUnique.Id { get; set; }

    /// <summary>
    /// The height of the 'center of this biome.
    /// Used to smooth biomes into surrounding biomes with an overall heightmap.
    /// </summary>
    public int BaseHeight {
      get;
      protected internal set;
    }

    /// <summary>
    /// The tile board this biome is a part of
    /// </summary>
    public Tile.Board Board {
      get;
    }

    /// <summary>
    /// The vornoi polygons this biome is made of
    /// TODO: serialize to their ids
    /// </summary>
    public IEnumerable<Polygon> Polygons 
      => _polygons;

    Biome(IBuilder<Biome> builder) {
      Board = builder.GetAndValidateParamAs<Tile.Board>(nameof(Tile.Board));
      BaseHeight = builder.GetAndValidateParamAs<int>(nameof(BaseHeight));
      _polygons = builder.GetAndValidateParamAs<Polygon>(nameof(Polygon)).AsSingleItemEnumerable().ToList();
      (builder.Archetype as Biome.Type).InitializeBiomeForCell(this, Polygons.First());
    }

    internal void _addPolygon(Polygon polygon) {
      _polygons.Add(polygon);
    }
  }
}
