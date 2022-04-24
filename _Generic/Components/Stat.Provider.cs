using Meep.Tech.Data;
using System;

namespace SpiritWorlds.Data.Components {

  /// <summary>
  /// Provides different kinds of stats
  /// </summary>
  public class StatsProvider : IModel.IComponent<StatsProvider>, IComponent.IUseDefaultUniverse {
    public IStat Get(Stat.Type type) {
      throw new NotImplementedException();
    }
  }
}
