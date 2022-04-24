using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data.Components {

  /// <summary>
  /// Potential item drops.
  /// Attaches to archetyps for things like features and entities
  /// </summary>
  public class PotentialItemDrops
    : Archetype.IComponent<PotentialItemDrops>,
      IComponent.IUseDefaultUniverse {

    /// <summary>
    /// The values in this potential item drops collection
    /// </summary>
    public IEnumerable<DropWithThreshhold> Values {
      get;
    }

    /// <summary>
    /// Make a new set of potential item drops.
    /// </summary>
    public static PotentialItemDrops Make(params DropWithThreshhold[] drops)
      => Components<PotentialItemDrops>.BuilderFactory.Make((nameof(Values), drops));

    /// <summary>
    /// Roll for a random set of drops.
    /// </summary>
    /// <param name="parentModel">The thing dropping these items</param>
    /// <param name="limit">(optional) A limit on how many items to get back.</param>
    /// <param name="differentRandomValueForEachAttempt">If the random threshhold value should be changed between item drop attempts</param>
    /// <param name="extraActivationContexts">Extra contexts related to the reason/action causing the parent model to drop items</param>
    /// <returns>A random collection of item drops given the parent, and other contexts</returns>
    public IEnumerable<ValueStack<Item>> RollForDropsFrom(IReadableComponentStorage parentModel, uint ? limit = null, bool differentRandomValueForEachAttempt = true, IEnumerable<object> extraActivationContexts = null) {
      double? threshold = null;
      int count = 0;
      List<ValueStack<Item>> results = new();
      foreach (DropWithThreshhold dropThreshhold in Values) {
        if (threshold == null || differentRandomValueForEachAttempt) {
          threshold = Math.Round(Meep.Tech.Noise.RNG.Static.NextDouble(), 2);
        }

        if (dropThreshhold.TryToGetResult((float)(threshold.Value), out var result, parentModel, extraActivationContexts)) {
          results.Add(result.Value);
          count++;
          if (limit.HasValue && count >= limit) {
            return results;
          }
        }
        else continue;
      }

      return results;
    }

    /// <summary>
    /// Get all the drops where the threshold is met.
    /// </summary>
    /// <param name="parentModel">The thing dropping these items</param>
    /// <param name="threshhold">The threshold to provide for items to drop</param>
    /// <param name="limit">(optional) A limit on how many items to get back.</param>
    /// <param name="extraActivationContexts">Extra contexts related to the reason/action causing the parent model to drop items</param>
    /// <returns>A collection of items dropped given the threshhold, parent, and other contexts</returns>
    public IEnumerable<ValueStack<Item>> GetDropsFrom(IReadableComponentStorage parentModel, float threshhold = 1, uint? limit = null, IEnumerable<object> extraActivationContexts = null) {
      if (limit.HasValue) {
        int count = 0;
        List<ValueStack<Item>> results = new();
        foreach (DropWithThreshhold dropThreshhold in Values) {
          if (dropThreshhold.TryToGetResult(threshhold, out var result, parentModel, extraActivationContexts)) {
            results.Add(result.Value);
            count++;
            if (count >= limit) {
              return results;
            }
          }
          else continue;
        }

        return results;
      } else {
        return Values
          .Select(e => e.TryToGetResult(threshhold, out var result, parentModel, extraActivationContexts) ? result : null)
          .Where(e => e.HasValue).Select(e => e.Value);
      }
    }

    #region Xbam Config

    PotentialItemDrops(IBuilder<PotentialItemDrops> builder) {
      Values = builder.GetParam(nameof(Values), Enumerable.Empty<DropWithThreshhold>());
    }

    #endregion

    /// <summary>
    /// An item drop with a drop chance.
    /// </summary>
    public struct DropWithThreshhold {

      /// <summary>
      /// Get an item to drop, using the parent model and extra activation contexts.
      /// </summary>
      /// <param name="parentModel">The thing dropping this item. Can be null. If this value is null it's likely for the purposes of getting a default item for reporting all the drop types in a collection.</param>
      /// <param name="extraActivationContexts">Extra context objects passed in during the event that caused items to drop. Can be null or empty.</param>
      /// <returns>An item to drop if the drop chance is met</returns>
      public delegate ValueStack<Item> GetItemLogic(IReadableComponentStorage parentModel = null, IEnumerable<object> extraActivationContexts = null);

      /// <summary>
      /// Used to get the drop threshhold/rarity of an item, the provided threshold must meet this threshold for the item to drop.
      /// </summary>
      /// <param name="parentModel">The thing dropping this item. Can be null. If this value is null it's likely for the purposes of getting a default percentage for reporting.</param>
      /// <param name="extraActivationContexts">Extra context objects passed in during the event that caused items to drop. Can be null or empty.</param>
      /// <returns>the minimum theshold required for an item to be dropped</returns>
      public delegate float GetDropThresholdLogic(IReadableComponentStorage parentModel = null, IEnumerable<object> extraActivationContexts = null);

      /// <summary>
      /// The logic used to get the item.
      /// </summary>
      public GetItemLogic GetItem { get; }

      /// <summary>
      /// The logic used to get the item's drop threshold.
      /// </summary>
      public GetDropThresholdLogic GetDropThreshold  { get; }

      public DropWithThreshhold(GetItemLogic getItem, GetDropThresholdLogic getDropThreshhold) {
        GetItem = getItem;
        GetDropThreshold = getDropThreshhold;
      }

      public void Deconstruct(out GetItemLogic getItem, out GetDropThresholdLogic getDropThreshhold) {
        getItem = GetItem;
        getDropThreshhold = GetDropThreshold;
      }

      public static implicit operator (GetItemLogic getItem, GetDropThresholdLogic getDropThreshhold)(DropWithThreshhold value) {
        return (value.GetItem, value.GetDropThreshold);
      }

      public static implicit operator DropWithThreshhold((GetItemLogic getItem, GetDropThresholdLogic getDropThreshhold) value) {
        return new DropWithThreshhold(value.getItem, value.getDropThreshhold);
      }

      public static implicit operator DropWithThreshhold((Func<IReadableComponentStorage, IEnumerable<object>, ValueStack<Item>> getItem, Func<IReadableComponentStorage, IEnumerable<object>, float> getDropThreshhold) value) {
        return new DropWithThreshhold((x, y) => value.getItem(x, y), (x, y) => value.getDropThreshhold(x, y));
      }

      /// <summary>
      /// Try to get the resulting item, given a provided threshold.
      /// </summary>
      /// <param name="threshold">If the returned drop chance for the item is below or equal to the threshold, the item is returned.</param>
      /// <param name="resultingItem">The resultint item if the threshhold was met</param>
      /// <param name="parentModel">The thing dropping this item. Can be null. If this value is null it's likely for the purposes of getting a default percentage for reporting.</param>
      /// <param name="extraActivationContexts">Extra context objects passed in during the event that caused items to drop. Can be null or empty.</param>
      /// <returns></returns>
      public bool TryToGetResult(float threshold, out ValueStack<Item>? resultingItem, IReadableComponentStorage parentModel = null, IEnumerable<object> extraActivationContexts = null) {
        float luckRequired = GetDropThreshold(parentModel, extraActivationContexts);
        if (luckRequired <= threshold) {
          resultingItem = GetItem(parentModel, extraActivationContexts);
          return true;
        } else {
          resultingItem = null;
          return false;
        }
      }
    }
  }
}

