using Meep.Tech.Data;

namespace SpiritWorlds.Data {
  namespace Components {
    public static partial class Entities {
      /// <summary>
      /// A component containing the history of an entity.
      /// </summary>
      public class History
        : IModel.IComponent<History>,
          IModel.IComponent.IIsRestrictedTo<Entity>,
          IComponent.IUseDefaultUniverse {

        /// <summary>
        /// The cannonical birthdate/moment of creation for this entity
        /// (This is not a timestamp for when the entity was created in real world time)
        /// </summary>
        public Scape.Moment MomentOfCreation
          => throw new System.NotImplementedException();

        /// <summary>
        /// The cannonical age of this entity in years.
        /// </summary>
        public float Age
          => throw new System.NotImplementedException();

        /// <summary>
        /// Age up an entity.
        /// </summary>
        public void AgeUp(float timeToAddInYears)
          => throw new System.NotImplementedException();
      }
    }

    public static class EntityHistoryExtensions {

      /// <summary>
      /// Get the cannonical age of the given entity.
      /// -1 means the age isn't tracked.
      /// </summary>
      public static float GetAge(this Entity entity) 
        => entity.TryToGetComponent<Components.Entities.History>(out var found)
          ? found.Age
          : -1;
    }
  }
}
