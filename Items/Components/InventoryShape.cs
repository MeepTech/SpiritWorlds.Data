using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Geometry;
using SpiritWorlds.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data.Components {

  /// <summary>
  /// A the shape (made of slots) of this item in a shaped inventory.
  /// determined by a 3D grid with 0,0 being bottom left
  /// O is used to mark the center/pivot of the shaped icon, X is used for other parts. All other chars are ignored.
  /// </summary>
  public partial class InventoryShape
    : Archetype.IComponent<InventoryShape>,
      Archetype.IComponent.IIsRestrictedTo<Item.Type>,
      IEnumerable<InventoryShape.Blocks[]>,
      IComponent.IUseDefaultUniverse 
  {
    Blocks[,] _shapeBlocks;

    /// <summary>
    /// The default shape of an item in a shaped inventory.
    /// </summary>
    public static readonly InventoryShape DefaultShape
      = new InventoryShape(new[,] {
          {InventoryShape.Blocks.Pivot }
      });

    /// <summary>
    /// The shape blocks used to make up the item shape
    /// </summary>
    public enum Blocks {
      Empty,
      Pivot,
      Solid
    }

    /// <summary>
    /// Get the size of the shape's dimensions
    /// </summary>
    public (int width, int height) Size
      => (_shapeBlocks.GetLength(0), _shapeBlocks.GetLength(1));

    /// <summary>
    /// The pivot of the shape. Where it's centered in inventory manipulation
    /// </summary>
    public (int x, int y) Pivot {
      get => _pivot ??= FindPivot(_shapeBlocks);
    }
    (int x, int y)? _pivot;

    /// <summary>
    /// The bounding box of the shape
    /// </summary>
    public (int width, int height) BoundingBox {
      get {
        int largestDimension = Math.Max(Size.width, Size.height);
        return (largestDimension, largestDimension);
      }
    }

    /// <summary>
    /// Get a block by X,Y
    /// </summary>
    public Blocks this[int x, int y] {
      get {
        try {
          return _shapeBlocks[x, y];
        } catch (IndexOutOfRangeException e) {
          throw new IndexOutOfRangeException($"No block found in the shape at the requested index: {new Coordinate(x, y)}\n {e}");
        }
      }
    }

    /// <summary>
    /// Make one out of blocks
    /// 0,0 being bottom left
    /// </summary>
    public InventoryShape(Blocks[,] shapeBlocks = null) {
      _shapeBlocks = shapeBlocks ?? DefaultShape._shapeBlocks;
      _pivot = FindPivot(_shapeBlocks);
    } InventoryShape(IBuilder<InventoryShape> builder) 
      : this(builder.GetParam<Blocks[,]>(nameof(Blocks), null)) {}

    /// <summary>
    /// Get the bitflag int for this shape
    /// </summary>
    /// <returns></returns>
    public int GetSolidBlockBitflags() {
      int solidBlockFlags = 0;
      int blockIndex = 0;
      (int shapeWidth, int shapeHeight) = Size;
      for (int x = 0; x < 3; x++) {
        // if we're not in bounds it's not solid
        if (x >= shapeWidth) {
          break;
        } else {
          for (int y = 0; y < 3; y++) {
            // if we're not in bounds it's not solid
            if (y >= shapeHeight) {
              break;
            } else if (_shapeBlocks[x, y] != Blocks.Empty) {
              solidBlockFlags = solidBlockFlags.TurnBitOn(++blockIndex);
            }
          }
        }
      }

      return solidBlockFlags;
    }

    /// <summary>
    /// Do something on each part of the shape given it's location offset from the pivot and block type
    /// </summary>
    /// <param name="doWithEachPivotOffsetUntilFalse"></param>
    public void ForEachSlot(Func<(int x, int y), Blocks, bool> doWithEachPivotOffsetUntilFalse) {
      Coordinate.Zero.Until(BoundingBox, shapeCornerOffset => {
        if (shapeCornerOffset.IsWithin(Size)) {
          return doWithEachPivotOffsetUntilFalse(
            shapeCornerOffset - Pivot,
            _shapeBlocks[shapeCornerOffset.X, shapeCornerOffset.Y]
          );
        }

        return true;
      });
    }

    /// <summary>
    /// Do something on each part of the shape given it's location offset from the pivot and block type
    /// </summary>
    /// <param name="doWithEachPivotOffsetUntilFalse"></param>
    public void ForEachSlot(Action<Coordinate, Blocks> doWithEachSlot) {
      Coordinate.Zero.Until(BoundingBox, shapeCornerOffset => {
        if (shapeCornerOffset.IsWithin(Size)) {
          doWithEachSlot(
            shapeCornerOffset - Pivot,
            _shapeBlocks[shapeCornerOffset.X, shapeCornerOffset.Y]
          );
        }
      });
    }

    /// <summary>
    /// Make one out of blocks
    /// </summary>
    public static implicit operator InventoryShape(Blocks[,] shapeBlocks) 
      => new InventoryShape(shapeBlocks);

    public override string ToString() {
      string[] rows = new string[] { "", "", "" };
      int longestSideOfShape = Math.Max(_shapeBlocks.GetLength(0), _shapeBlocks.GetLength(1));
      Coordinate.Zero.Until((longestSideOfShape, longestSideOfShape), offset => {
        rows[offset.Y] += offset.IsWithin(Size)
          ? _shapeBlocks[offset.X, offset.Y] == Blocks.Solid
            ? "■"
            : _shapeBlocks[offset.X, offset.Y] == Blocks.Pivot
              ? "▣"
              : "□"
          : "□";
      });

      return string.Join("\n", rows.Reverse());
    }

    /// <summary>
    /// Find a shape's pivot.
    /// </summary>
    static (int x, int y) FindPivot(Blocks[,] shapeBlocks) {
      for (int x = 0; x < shapeBlocks.GetLength(0); x++) {
        for (int y = 0; y < shapeBlocks.GetLength(1); y++) {
          if (shapeBlocks[x, y] == Blocks.Pivot) {
            return (x, y);
          }
        }
      }

      return (0, 0);
    }

    public IEnumerator<Blocks[]> GetEnumerator() {
      int rows = _shapeBlocks.GetLength(1);
      while (--rows > 0) {
        int columns = _shapeBlocks.GetLength(1);
        Blocks[] row = new Blocks[columns];
        while (--columns > 0) {
          row[columns] = _shapeBlocks[rows, columns];
        }

        yield return row;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }

  public static class ItemExtensions {

    /// <summary>
    /// Get the slot quantity of an inventory item.
    /// Items = 1,
    /// Box or Stacks can have other quantities.
    /// </summary>
    public static InventoryShape GetShape(this Item item)
      => item.Archetype.GetComponent<InventoryShape>() ?? InventoryShape.DefaultShape;
  }
}
