using Meep.Tech.Data;
using System.Collections.Generic;

namespace SpiritWorlds.Data {

  /// <summary>
  /// A tag, for identifying and labeling data.
  /// </summary>
  public class Tag : Enumeration<Tag> {
    static readonly Dictionary<string, Tag> _withExtraContext = new();

    /// <summary>
    /// Make a new tag.
    /// </summary>
    public Tag(string key, Universe universe = null)
      : base(key, universe) { }

    /// <summary>
    /// Make a version of this tag with some required extra context.
    /// Can be used to make specific tags like 'level-up|[CHARACTERID]' vs just 'level-up'
    /// </summary>
    public Tag WithExtraContext(params string[] extraContexts) {
      string key = ExternalId as string + string.Join('|', extraContexts);
      return _withExtraContext.TryGetValue(key, out Tag existing)
        ? existing
        : (_withExtraContext[key] = new(key, Universe));
    }
  }
}
