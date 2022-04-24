namespace SpiritWorlds.Data {
  public partial struct Tile {

    /// <summary>
    /// A solid column of tiles of the same type.
    /// Used within a "Stack"
    /// </summary>
    public partial struct Column {

      /// <summary>
      /// The base tile that this is a stack of.
      /// Null is empty space. Usually empty space can be represented without a column though.
      /// </summary>
      public Tile? Type {
        get;
      }

      /// <summary>
      /// How many tiles deep/tall/high this column is.
      /// </summary>
      public int Height {
        get;
      }

      /// <summary>
      /// The height index location of the top of stack within it's column.
      /// </summary>
      public int BaseHeightLocation {
        get;
      }

      /// <summary>
      /// The height index location of the base/bottom of stack within it's column.
      /// </summary>
      public int TopHeightLocation
        => BaseHeightLocation + Height;

      /// <summary>
      /// Make a stack of tiles.
      /// </summary>
      public Column(Tile? type = null, int? height = 1, int? heightIndexOfColumnBaseWithinParentStack = 0) {
        Type = type;
        if (height.HasValue && height < 1) {
          throw new System.ArgumentException($"Columns must have a height equal to or greater than 1");
        }
        Height = height ?? 1;
        if (heightIndexOfColumnBaseWithinParentStack.HasValue && heightIndexOfColumnBaseWithinParentStack < 0) {
          throw new System.ArgumentException($"Columns must have a height equal to or greater than 0");
        }
        BaseHeightLocation = heightIndexOfColumnBaseWithinParentStack ?? 0;
      }
    }
  }
}
