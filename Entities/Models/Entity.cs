using Meep.Tech.Data;

namespace SpiritWorlds.Data {

  /// <summary>
  /// The Base Model for all Entitys. Entities are in world objects that can be interacted with/seen etc.
  /// </summary>
  public partial class Entity 
    : Model<Entity, Entity.Type>.WithComponents,
      IModel.IUseDefaultUniverse,
      ICached<Entity>
  {

    #region Model Data Fields

    /// <summary>
    /// The unique id of this entity.
    /// </summary>
    public string Id {
      get;
      private set;
    } string IUnique.Id {
      get => Id;
      set => Id = value;
    }

    /// <summary>
    /// The custom name of this entity.
    /// </summary>
    public virtual string Name {
      get;
      protected set;
    }

    #endregion

    /// <summary>
    /// Used to make new item types.
    /// </summary>
    /// <param name="builder"></param>
    protected Entity(IBuilder<Entity> builder) : this() {
      Name ??= builder.GetParam(nameof(Name), Archetype.Id.Name);
    } Entity() : base() { }

    #region Equality and Conversion

    public override bool Equals(object obj)
      => Id == (obj as Item)?.Id;

    public override string ToString()
      => $"{Name}{(Name == Archetype.Id.Name ? "" : $" ({Archetype.Id.Name})")}";

    public override int GetHashCode()
      => Id.GetHashCode();

    #endregion
  }
}
