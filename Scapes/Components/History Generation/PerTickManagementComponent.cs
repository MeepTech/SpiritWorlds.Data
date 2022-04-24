using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  public partial class Scape {
    public partial class History {
      public partial class Generator {

        /// <summary>
        /// A part of a generator that does something each tick.
        /// (Extend the generic version instead)
        /// </summary>
        public abstract class PerTickManagementComponent
          : IModel.IComponent,
            IComponent.IUseDefaultUniverse {

          /// <summary>
          /// The parent generator for this component.
          /// </summary>
          public Generator Generator {
            get;
          }

          /// <summary>
          /// For XBam
          /// </summary>
          protected PerTickManagementComponent(IBuilder builder)
            => Generator = builder.Parent as Generator;
          /// <summary>For Json</summary>
          protected PerTickManagementComponent() { }

          /// <summary>
          /// Process the history for the current tick of world time.
          /// </summary>
          internal abstract protected History ProcessForCurrentTick(History currentHistory);
        }

        /// <summary>
        /// A part of a generator that does something each tick.
        /// </summary>
        public abstract class PerTickManagementComponent<TComponentBase>
            : PerTickManagementComponent,
              IModel.IComponent.IIsRestrictedTo<Scape.History.Generator>,
              IModel.IComponent<TComponentBase>
          where TComponentBase : IModel.IComponent<TComponentBase> {

          /// <summary>
          /// For XBam
          /// </summary>
          protected PerTickManagementComponent(IBuilder builder)
            : base(builder) { }
          protected PerTickManagementComponent() : base() { }
        }
      }
    }
  }
}
