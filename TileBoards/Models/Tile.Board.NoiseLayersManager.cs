using Meep.Tech.Noise;
using System.Collections;
using System.Collections.Generic;

namespace SpiritWorlds.Data {
  public partial struct Tile {
    public partial class Board {

      /// <summary>
      /// Can be used to manage and request cached noise layers by their key.
      /// This is basically just a dictionary that creates a new entry for a requested layer that does not exist.
      /// </summary>
      public class NoiseLayersManager : IReadOnlyDictionary<NoiseLayer, FastNoise> {
        Dictionary<NoiseLayer, FastNoise> _layers;
        readonly int _seed;

        public IEnumerable<NoiseLayer> Keys
          => ((IReadOnlyDictionary<NoiseLayer, FastNoise>)_layers).Keys;

        public IEnumerable<FastNoise> Values
          => ((IReadOnlyDictionary<NoiseLayer, FastNoise>)_layers).Values;

        public int Count
          => ((IReadOnlyCollection<KeyValuePair<NoiseLayer, FastNoise>>)_layers).Count;

        public FastNoise this[NoiseLayer key]
          => ((IReadOnlyDictionary<NoiseLayer, FastNoise>)_layers).TryGetValue(key, out var found)
            ? found
            : (_layers[key] = new FastNoise(_seed));

        internal NoiseLayersManager(int scapeSeed) { _seed = scapeSeed; }

        public bool ContainsKey(NoiseLayer key)
          => ((IReadOnlyDictionary<NoiseLayer, FastNoise>)_layers).ContainsKey(key);

        public bool TryGetValue(NoiseLayer key, out FastNoise value)
          => ((IReadOnlyDictionary<NoiseLayer, FastNoise>)_layers).TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<NoiseLayer, FastNoise>> GetEnumerator()
          => ((IEnumerable<KeyValuePair<NoiseLayer, FastNoise>>)_layers).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
          => ((IEnumerable)_layers).GetEnumerator();
      }
    }
  }
}