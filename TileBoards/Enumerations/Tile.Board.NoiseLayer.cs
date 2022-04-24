using Meep.Tech.Data;
using Meep.Tech.Noise;
using System;

namespace SpiritWorlds.Data {

  public partial struct Tile {
    public partial class Board {
      /// <summary>
      /// A named layer of noise used for biome generation.
      /// </summary>
      public class NoiseLayer : Enumeration<NoiseLayer> {

        /// <summary>
        /// The default method of making a noise layer's noise object
        /// </summary>
        public static Func<FastNoise> DefaultNoiseGenerator
          = () => new FastNoise();

        /// <summary>
        /// Make a new noise layer type
        /// </summary>
        /// <param name="nameKey">Unique name</param>
        /// <param name="noiseGeneratorOverride">(optional) the function to use to make the new noise layer. Defaults to DefaultNoiseGenerator: () => new FastNoise();</param>
        public NoiseLayer(string nameKey, System.Func<Meep.Tech.Noise.FastNoise> noiseGeneratorOverride = null)
          : base(nameKey) {
          GetNoise = noiseGeneratorOverride ?? DefaultNoiseGenerator;
        }

        public Func<FastNoise> GetNoise { get; }
      }
    }
  }
}
