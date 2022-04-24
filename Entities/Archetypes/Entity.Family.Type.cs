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

        /// <summary>
        /// How far appart two entities must be in the family tree (at minimum) to fall in love or have a kid.
        /// 1 would be sibling/parent, 2 would be cousin/pibling/grandchild/grandparent, 3 is 1st cousin, once removed pibbling, great-grand etc~.
        /// </summary>
        public virtual int MinimumFamilialDistanceRequiredForLove {
          get;
        } = 3;

        protected Type(Archetype.Identity id)
          : base(id ?? Base) { }

        /// <summary>
        /// Make a new family of entities.
        /// </summary>
        public virtual Family Make(string name = null)
          => Make<Family>((nameof(Name), name));

        /// <summary>
        /// The logic for generating a new family member with this family type.
        /// </summary>
        /// <param name="family"></param>
        /// <param name="seedBasedRandomizer"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected internal virtual Entity GenerateNextNewFamilyMember(Family family) {
          // option 1, check new baby chance among couples in this family.
          if (family.Couples.Any() && family.SeedBasedRandomizer.NextDouble() < 0.65f) {
            
          }
          
          // option 2, check if an adult likes someone enough to adopt them. Being in a couple ups the chance here.
          if (family.SeedBasedRandomizer.NextDouble() < 0.35f) {

          }

          // option 3, add someone random to the family from somewhere else if we didn't add a baby to the tree.
          /// check if we have relatives
          
          /// no relatives? this must be the first family member


        }
      }
    }
  }
}