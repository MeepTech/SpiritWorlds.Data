using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Newtonsoft.Json;

namespace SpiritWorlds.Data {

  /// <summary>
  /// The Base Model for all Items
  /// </summary>
  public partial class Item
    : Model<Item, Item.Type>.WithComponents,
      IModel.IUseDefaultUniverse,
      IKeyedStackable,
      ICached<Item> 
  {

    #region Model Data Fields

    /// <summary>
    /// The unique id of this item.
    /// </summary>
    public string Id {
      get;
      private set;
    } string IUnique.Id {
      get => Id;
      set => Id = value;
    }

    /// <summary>
    /// The custom name of this item.
    /// </summary>
    public virtual string Name {
      get;
      protected set;
    }

    #endregion

    #region IStackable

    [JsonIgnore]
    public virtual string StackKey
      => Archetype.GetStackKey(this);

    [JsonIgnore]
    public virtual int? MaxQuantityPerStack
      => Archetype.MaxQuantityPerStack;

    #endregion

    /// <summary>
    /// Used to make new item types.
    /// </summary>
    /// <param name="builder"></param>
    protected Item(IBuilder<Item> builder) : this() {
      Name ??= builder.GetParam(nameof(Name), Archetype.Id.Name);
    } Item() : base() { }

    #region Equality and Conversion

    /// <summary>
    /// Check if this item can stack with another.
    /// This can be used to check equality of the item's contents/data rather than it's Id.
    /// </summary>
    public bool CanStackWith(Item otherItem)
      => (this as IKeyedStackable).CanStackWith(otherItem);

    public override bool Equals(object obj)
      => Id == (obj as Item)?.Id;

    public override string ToString()
      => $"{Name}{(Name == Archetype.Id.Name ? "" : $" ({Archetype.Id.Name})")}";

    public override int GetHashCode()
      => Id.GetHashCode();

    #endregion
  }
}
