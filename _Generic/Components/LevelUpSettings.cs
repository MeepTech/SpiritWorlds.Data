using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;

namespace SpiritWorlds.Data.Components {

  /// <summary>
  /// A component containing the basic combat stats used in spiritworlds.
  /// </summary>
  public class LevelUpSettings
    : Archetype.IComponent<LevelUpSettings>,
      Archetype.ILinkedComponent<Levels>,
      IComponent.IUseDefaultUniverse
  {

    /// <summary>
    /// The max level allowed.
    /// </summary>
    public int MaxLevel { 
      get;
    }
  
    /// <summary>
    /// Get the exp required to level up to the given level.
    /// </summary>
    public Func<int, double> GetExpRequiredToLevelUpTo {
      get;
    }

    /// <summary>
    /// Get the exp required to level up to the given level.
    /// </summary>
    public Func<IModel, LevelUpSettings, Levels> GetDefaultLevels {
      get;
    }

    /// <summary>
    /// Actions performed on level up success.
    /// Params: 
    /// - Provided stats for the level up  
    /// - The updated Levels data component
    /// </summary>
    public DelegateCollection<Action<IEnumerable<Stat.Type>, Levels>> DefaultOnLevelUpSuccessActions {
      get;
    } = new();

    /// <summary>
    /// Actions performed on level up failure.
    /// Params: 
    /// - Provided stats for the level up  
    /// - The Levels data component
    /// </summary>
    public DelegateCollection<Action<IEnumerable<Stat.Type>, Levels>> DefaultOnLevelUpFailedActions {
      get;
    } = new();

    /// <summary>
    /// The model component data.
    /// </summary>
    public Levels LinkedModelComponent {
      get;
      internal set;
    }

    /// <summary>
    /// copy everything and override the base functionality
    /// </summary>
    public LevelUpSettings CopyWithNewBaseFunctionality(Func<int, double> newGetRequiredLevelUpExpFunction = null, Func<IModel, LevelUpSettings, Levels> newGetDefaultLevelsFunc = null, int? maxLevel = null) {
      var copy = Make(newGetRequiredLevelUpExpFunction ?? GetExpRequiredToLevelUpTo, newGetDefaultLevelsFunc ?? GetDefaultLevels, maxLevel ?? MaxLevel);

      DefaultOnLevelUpSuccessActions.ForEach(action =>
        copy.DefaultOnLevelUpSuccessActions.Add(action.Key, action.Value));
      DefaultOnLevelUpFailedActions.ForEach(action =>
        copy.DefaultOnLevelUpFailedActions.Add(action.Key, action.Value));

      return copy;
    }

    /// <summary>
    /// Make a new set of level up settings.
    /// </summary>
    public static LevelUpSettings Make(Func<int, double> getExpRequiredToLevelUpTo, Func<IModel, LevelUpSettings, Levels> getDefaultLevelData = null, int? maxLevel = null)
      => Components<LevelUpSettings>.BuilderFactory.Make(
        (nameof(GetExpRequiredToLevelUpTo), getExpRequiredToLevelUpTo),
        (nameof(GetDefaultLevels), getDefaultLevelData),
        (nameof(MaxLevel), maxLevel)
      );

    #region XBam Config

    LevelUpSettings(IBuilder<LevelUpSettings> builder) {
      MaxLevel = builder.GetParam(nameof(MaxLevel), 50);
      GetExpRequiredToLevelUpTo = builder.GetAndValidateParamAs<Func<int, double>>(nameof(GetExpRequiredToLevelUpTo));
      GetDefaultLevels = builder.GetParam<Func<IModel, LevelUpSettings, Levels>>(nameof(GetDefaultLevels), null);
    }

    Levels Archetype.ILinkedComponent<Levels>.BuildDefaultModelComponent(IModel.Builder parentModelBuilder, Universe universe = null)
      => GetDefaultLevels?.Invoke(parentModelBuilder.Parent, this) ?? Levels.Make(parentModelBuilder.Parent, this);

    #endregion
  }
}