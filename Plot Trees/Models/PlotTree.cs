using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  public static partial class PlotTrees {
  }

  /// <summary>
  /// The Base Model for all PlotTrees
  /// </summary>
  public partial class PlotTree : Model<PlotTree, PlotTree.Type>, IModel.IUseDefaultUniverse, IUnique {
    
    /// <summary>
    /// The id of this plot tree
    /// </summary>
    public string Id {
      get;
      private set;
    } string IUnique.Id { 
      get => Id; 
      set => Id = value; 
    }

    /// <summary>
    /// Tags for this plot tree.
    /// </summary>
    public virtual IEnumerable<Tag> Tags
      => Archetype.DefaultTags;

    public Node ActiveNode { get; }

    /// <summary>
    /// The Base Archetype for PlotTrees
    /// </summary>
    public abstract class Type : Archetype<PlotTree, PlotTree.Type> {

      /// <summary>
      /// The type of the root node of this type of plot tree
      /// </summary>
      public Node.Type RootNodeType { get; }

      /// <summary>
      /// Overrideable default tags for plot trees of this type.
      /// </summary>
      public virtual IEnumerable<Tag> DefaultTags { get => _DefaultTags ?? Enumerable.Empty<Tag>(); }
      /** <summary> The backing field used to initialize and override the initail value of DefaultTags.
       * You can this syntax to override or add to the base initial value: 
       * '=> _DefaultTags ??= base.DefaultTags.Append(...' </summary> 
       **/
      protected IEnumerable<Tag> _DefaultTags {
        get => _defaultTags; set => _defaultTags = value;
      }
      IEnumerable<Tag> _defaultTags;

      /// <summary>
      /// Used to make new Child Archetypes for PlotTree.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      internal Type(Identity id)
        : base(id) { }
    }

    /// <summary>
    /// Progress the current node tree by forcing it to make it's next choice and continuing on to it's next node.
    /// </summary>
    public void Progress(Scape.History currentHistory, Scape.History.Generator generator) {
      throw new NotImplementedException();
    }
  }
}
