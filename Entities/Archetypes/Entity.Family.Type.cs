using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Linq;

namespace SpiritWorlds.Data {
  public partial class Entity {
    public partial class Family {

      /// <summary>
      /// An archetype for a type of family.
      /// </summary>
      public class Type : Archetype<Family, Type> {

        /// <summary>
        /// Id for the base type.
        /// </summary>
        public static Identity Base {
          get;
        } = new Identity("Default");

        protected Type(Archetype.Identity id)
          : base(id ?? Base) { }

        /// <summary>
        /// Make a new family of entities.
        /// </summary>
        public virtual Family Make(string name = null)
          => Make<Family>((nameof(Name), name));
      }
    }
  }
}