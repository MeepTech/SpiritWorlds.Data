using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {
  public partial struct Tile {
    public partial struct Column {

      /// <summary>
      /// A stack of tile towers from the ground up.
      /// </summary>
      public struct Stack : IEnumerable<Tile.Column> {
        readonly IEnumerable<Tile.Column> _columns;
        List<Tile.Column> _columnList
          => _compiledColumnList ?? _columns.ToList();
        List<Tile.Column> _compiledColumnList;

        /// <summary>
        /// Make a stack of tile columns.
        /// </summary>
        public Stack(IEnumerable<Column> columnsFromTheBottomUp = null) : this() {
          _columns = columnsFromTheBottomUp?.ToList();

          /// Validate that there are no overlapping columns.
          if (_columns is not null && _columns.Count() > 1) {
            Tile.Column last = _columns.First();

            foreach(Tile.Column current in _columns.Skip(1)) {
              int lastColumnTopLocation = last.TopHeightLocation;
              if (current.BaseHeightLocation <= lastColumnTopLocation) {
                throw new System.ArgumentException($"Overlapping columns: {last}, {current}");
              }
              last = current;
            }
          }
        }

        /// <summary>
        /// Make a stack of tile columns.
        /// </summary>
        /// <param name="columnsFromTheBottomUp">A list of data items to generate columns from. Null in baseHeightLocationIndex provides 0 (the bottom) and null in height provides the default stack height (1)</param>
        public static Stack Make(params (int? baseHeightLocationIndex, (Tile.Type.Identity type, int? height) column)[] columnsFromTheBottomUp)
          => new(columnsFromTheBottomUp.Select(c => (c.column.type.Archetype as Tile.Type).MakeColumn(c.column.height, c.baseHeightLocationIndex)));

        /// <summary>
        /// Get the tile in this stack at the given height location.
        /// </summary>
        public Tile? this[int tileHeightLocationFromBottom] {
          get {
            if (_columns == null || !_columns.Any()) {
              return null;
            }

            int columnIndex = 0;
            Tile.Column stack;
            int currentHeightInStack = tileHeightLocationFromBottom;

            do {
              if (columnIndex >= _columnList.Count) {
                return null;
              }

              stack = _columnList[columnIndex++];
              currentHeightInStack -= stack.BaseHeightLocation;
              if (currentHeightInStack < 0) {
                return null;
              }
            } while (currentHeightInStack > stack.Height);

            return stack.Type;
          }
        }

        ///<summary><inheritdoc/></summary>
        public IEnumerator<Column> GetEnumerator()
          => _columns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
          => GetEnumerator();
      }
    }
  }
}
