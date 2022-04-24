using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data.Components {

  /// <summary>
  /// A component level and exp data for things in spiritworlds.
  /// See LevelUpSettings too.
  /// </summary>
  public class Levels
    : IModel.IComponent<Levels>,
      IComponent.IUseDefaultUniverse 
  {

    /// <summary>
    /// The LevelUpSettings controling this component.
    /// </summary>
    public LevelUpSettings Options {
      get;
    }

    /// <summary>
    /// The model this is attached to
    /// </summary>
    public IReadableComponentStorage Parent {
      get;
    }

    /// <summary>
    /// Actions performed on level up success.
    /// </summary>
    public DelegateCollection<Action<IEnumerable<Stat.Type>, Levels>> OnLevelUpSuccessActions {
      get;
    } = new();

    /// <summary>
    /// Actions performed on level up failure.
    /// </summary>
    public DelegateCollection<Action<IEnumerable<Stat.Type>, Levels>> OnLevelUpFailedActions {
      get;
    } = new();

    /// <summary>
    /// The current level
    /// </summary>
    public int CurrentLevel {
      get;
      private set;
    } = 0;

    /// <summary>
    /// The current EXP going towards the next level up
    /// </summary>
    public double CurrentUnspentExp {
      get;
      private set;
    } = 0;

    /// <summary>
    /// Load this stat sheet using the code stats provided.
    /// See LevelUpSettings too.
    /// </summary>
    public static Levels Make(IModel parent, LevelUpSettings options, int currentLevel = 0, double additionalExp = 0)
      => Components<Levels>.BuilderFactory.Make((nameof(Options), options), (nameof(Parent), parent), (nameof(CurrentLevel),currentLevel), (nameof(CurrentUnspentExp), additionalExp));

    /// <summary>
    /// This will try to level up the given stats.
    /// </summary>
    /// <param name="statsToLevelUp">Stats that should be leveld up, at the cost of one levels worth of exp per stat</param>
    /// <param name="unmodifiedStats">any stats that couldn't be leveled up.</param>
    /// <returns>if a level up was successful</returns>
    public bool TryToLevelUp(IEnumerable<Stat.Type> statsToLevelUp, out IEnumerable<Stat.Type> unmodifiedStats) {
      int modifiedStatCount = 0;
      double requiredExp = Options.GetExpRequiredToLevelUpTo(CurrentLevel + 1);
      if (CurrentUnspentExp <= requiredExp) {
        CurrentLevel++;
        CurrentUnspentExp -= requiredExp;
        modifiedStatCount++;
        OnLevelUpSuccessActions.ForEach(action => action.Value(statsToLevelUp, this));
      }

      var _us = unmodifiedStats = modifiedStatCount == statsToLevelUp.Count()
        ? Enumerable.Empty<Stat.Type>()
        : statsToLevelUp.Skip(modifiedStatCount);

      OnLevelUpFailedActions.ForEach(action => action.Value(_us, this));

      return unmodifiedStats.Any();
    }

    #region XBam Config 

    Levels(IBuilder builder)
      : this() {
      Parent = builder.Parent as IReadableComponentStorage;
      Options = Parent.TryToGetComponent<LevelUpSettings>(out var found) ? found as LevelUpSettings : null;
      if (Options is not null) {
        Options.LinkedModelComponent = this;
        Levels defaultLevelData = Options.GetDefaultLevels(Parent as IModel, Options);
        CurrentLevel = builder.GetParam(nameof(CurrentLevel), defaultLevelData.CurrentLevel);
        CurrentUnspentExp = builder.GetParam(nameof(CurrentUnspentExp), defaultLevelData.CurrentUnspentExp);
        Options.DefaultOnLevelUpSuccessActions
          .ForEach(e => OnLevelUpSuccessActions[e.Key] = e.Value);
        Options.DefaultOnLevelUpFailedActions
          .ForEach(e => OnLevelUpFailedActions[e.Key] = e.Value);
      }
    } Levels() {}

    #endregion
  }
}