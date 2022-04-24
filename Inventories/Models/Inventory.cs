using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  /// <summary>
  /// The Base Model for all Inventoryies
  /// </summary>
  public abstract partial class Inventory
    : Model<Inventory, Inventory.Type>.IFromInterface,
      IModel.IUseDefaultUniverse 
  {

    /// <summary>
    /// The archetype used to construct this inventory
    /// </summary>
    public Type Archetype {
      get;
      private set;
    } Type Model<Inventory, Type>.IFromInterface.Archetype {
      get => Archetype;
      set => Archetype = value;
    }

    ///<summary><inheritdoc/></summary>
    public int Count
      => Values.Count;

    ///<summary><inheritdoc/></summary>
    public IEnumerable<Item.Type> ItemTypes
      => Values
        .Select(v => v?.Value.Archetype)
        .Where(a => a is not null)
        .Distinct();

    /// <summary>
    /// The items in this inventory.
    /// </summary>
    public IEnumerable<Item> Items
      => Values
        .Where(v => v.HasValue)
        .SelectMany(v => v.Value);

    ///<summary><inheritdoc/></summary>
    public IEnumerable<ValueStack<Item>?> Stacks
      => Values;

    /// <summary>
    /// An access collection of the stacked items
    /// </summary>
    protected StackedInventory<Item> Values {
      get;
    }

    ///<summary><inheritdoc/></summary>
    public IEnumerable<ValueStack<Item>> this[Item.Type type]
      => Values
        .Where(i => i?.Value.Archetype == type)
        .Select(v => v.Value);

    /// <summary>
    /// Used to make a new type of inventory.
    /// </summary>
    protected Inventory(IBuilder builder) {
      if (builder.TryToGetParam(nameof(Stacks), out IEnumerable<ValueStack<Item>> stacks)) {
        LoadFromStacks(stacks);
      } else if (builder.TryToGetParam(nameof(Items), out IEnumerable<Item> items)) {
        LoadFromItems(items);
      }
    }

    /// <summary>
    /// Load this from a set of items
    /// </summary>
    /// <param name="items"></param>
    protected void LoadFromItems(IEnumerable<Item> items) {
      items.ForEach(i => Add(i));
    }

    /// <summary>
    /// Load this from a set of stacks.
    /// </summary>
    protected void LoadFromStacks(IEnumerable<ValueStack<Item>> stacks) {
      stacks.ForEach(s => Add(s, out _));
    }

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <returns>if the item was added successfully.</returns>
    public bool Add(Item item)
      => Add(item, out _);

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <param name="count">(optional) how many copies of this item to add</param>
    /// <param name="remainder">(out) How many items couldn't be added if any were left over</param>
    /// <returns>if the item was added successfully.</returns>
    public bool Add(Item item, out int remainder, int count = 1)
      => Add(new(item, count), out remainder);

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <param name="count">(optional) how many copies of this item to add</param>
    /// <param name="remainder">(out) How many items couldn't be added if any were left over</param>
    /// <returns>if the item was added successfully.</returns>
    public bool Add(Item item, int count)
      => Add(new(item, count), out _);

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <returns>if the item was added successfully.</returns>
    public abstract bool Add(ValueStack<Item> items, out int remainder);

    ///<summary><inheritdoc/></summary>
    public bool Contains(Item.Type type)
      => Values.Any(i => i?.Value.Archetype == type);

    ///<summary><inheritdoc/></summary>
    public bool Contains(Item value)
      => Values[value.StackKey].Any();
  }

  /// <summary>
  /// The base for inventories with specific slots.
  /// </summary>
  public abstract partial class Inventory<TSlotKey> :
    Inventory
    where TSlotKey : struct
  {

    ///<summary><inheritdoc/></summary>
    public abstract IEnumerable<KeyValuePair<TSlotKey, ValueStack<Item>>> Slots {
      get;
    }

    ///<summary><inheritdoc/></summary>
    public abstract IEnumerable<ValueStack<Item>> this[TSlotKey key] {
      get;
    }

    ///<summary><inheritdoc/></summary>
    protected Inventory(IBuilder builder) : base(builder) {
      if (Count == 0 && builder.TryToGetParam(nameof(Slots), out IEnumerable<KeyValuePair<TSlotKey, ValueStack<Item>>> slots)) {
        slots.ForEach(s => Add(s.Value, s.Key, out _));
      }
    }

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <returns>if the item was added successfully.</returns>
    public abstract bool Add(ValueStack<Item> items, TSlotKey? slot, out int remainder);

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <returns>if the item was added successfully.</returns>
    public abstract bool Swap(ValueStack<Item> items, out ValueStack<Item> existingItem, out int remainder, TSlotKey? slot);

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <returns>if the item was added successfully.</returns>
    public bool Add(Item item, TSlotKey? slot)
      => Add(item, slot, out _);

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <param name="count">(optional) how many copies of this item to add</param>
    /// <param name="remainder">(out) How many items couldn't be added if any were left over</param>
    /// <returns>if the item was added successfully.</returns>
    public bool Add(Item item, TSlotKey? slot, out int remainder, int count = 1)
      => Add(new ValueStack<Item>(item, count), slot, out remainder);

    ///<summary><inheritdoc/></summary>
    public override bool Add(ValueStack<Item> items, out int remainder)
      => Add(items, null, out remainder);

    /// <summary>
    /// Swap the provided item with an existing item in the inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">(optional) a target slot for the item</param>
    /// <param name="existingItem">(out) The item that was in that slot if there was one</param>
    /// <returns>if anything was swaped successfully and the item was placed in it's expected slot</returns>
    public bool Swap(Item item, TSlotKey? slot, out ValueStack<Item> existingItem)
      => Swap(item, slot, out existingItem, out _);

    /// <summary>
    /// Add an item to this inventory.
    /// </summary>
    public bool Swap(Item item, TSlotKey? slot, out ValueStack<Item> existingItem, out int remainder,  int count = 1)
      => Swap(new ValueStack<Item>(item, count), out existingItem, out remainder, slot);
  }
}
