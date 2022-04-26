using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using SpiritWorlds.Data.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {
  public partial class Entity {

    /// <summary>
    /// A family made of entities.
    /// TODO: does this need to be an xbam model?
    /// </summary>
    public partial class Family : Model<Family, Family.Type> {

      /// <summary>
      /// Represents a family connection.
      /// </summary>
      public struct Relationship {

        /// <summary>
        /// The root entity for this relationship
        /// </summary>
        public Entity CurrentEntity { get; }

        /// <summary>
        /// The type of relationship that the CurrentEntity has to the Relative.
        /// The CurrentEntity is the [Relation] of the Relative
        /// </summary>
        public Relations Relation { get; }

        /// <summary>
        /// The other member that the CurrentEntity is related to.
        /// </summary>
        public Entity Relative { get; }

        internal Relationship(Entity entity, Relations relation, Entity relative) {
          Relation = relation;
          CurrentEntity = entity;
          Relative = relative;
        }
      }

      /// <summary>
      /// Direct relationships between family members.
      /// </summary>
      public enum Relations {
        Parent,
        Child,
        Sibling,
        Spouse
      }

      /// <summary>
      /// The family name
      /// </summary>
      public string Name {
        get;
      }

      /// <summary>
      /// The randomizer used to make this family.
      /// </summary>
      protected Random SeedBasedRandomizer {
        get;
      }

      /// <summary>
      /// The root/original/first member of this family tree
      /// </summary>
      public Entity RootMember {
        get;
        private set;
      }

      /// <summary>
      /// All the members of this family.
      /// </summary>
      public ICollection<Entity> Members
        => _relations.Keys;

      /// <summary>
      /// The couples in this family.
      /// </summary>
      public IReadOnlyMap<Entity, Entity> Couples 
        => _couples; readonly Map<Entity, Entity> _couples 
        = new();

      /// <summary>
      /// The relationships between entities.
      /// </summary>
      Dictionary<Entity, Dictionary<Relations, List<Entity>>> _relations
        = new();

      /// <summary>
      /// Get the direct relationships for the given entity.
      /// </summary>
      public IEnumerable<Relationship> GetDirectRelationships(Entity entity, Relations? relationshipType = null) {
        if (_relations.TryGetValue(entity, out var relationsData)) {
          if (relationshipType.HasValue) {
            return relationsData.TryToGet(relationshipType.Value)?
              .Select(e => new Relationship(entity, relationshipType.Value, e)) 
                ?? Enumerable.Empty<Relationship>();
          }
          else {
            return relationsData.SelectMany(e =>
              e.Value.Select(ee => new Relationship(entity, e.Key, ee)));
          }
        }

        return Enumerable.Empty<Relationship>();
      }

      /// <summary>
      /// Start the family with the given parent/couple as a root.
      /// </summary>
      public virtual void Start(Entity rootParent, Entity routeSpouse = null) {
        RootMember = rootParent;
        _relations[RootMember] = new();
        RootMember._addToFamily(this);

        if (routeSpouse != null) {
          AddFamilyMember(routeSpouse, Relations.Spouse, RootMember);
        }
      }

      /// <summary>
      /// Add a new member to a family.
      /// (optional) you can provide a relationship with an existing family member already, if not, one will be generated.
      /// </summary>
      public virtual Relationship AddFamilyMember(Entity newFamilyMember, Relations? asRelationship = null, Entity ofExistingFamilyMember = null) {
        bool relationshipIsMaleable = false;
        if (asRelationship is null) {
          relationshipIsMaleable = true;
          asRelationship = Enum.GetValues(typeof(Family.Relations)).Cast<Family.Relations>().RandomEntry(SeedBasedRandomizer);
        }

        if (ofExistingFamilyMember is null) {
          ofExistingFamilyMember = Members.RandomEntry(SeedBasedRandomizer);
          /// Make sure ages make sense.
          if (asRelationship == Relations.Child) {
            if (newFamilyMember.GetAge() > ofExistingFamilyMember.GetAge()) {
              ofExistingFamilyMember = Members.Where(fm =>
                fm.GetAge() > newFamilyMember.GetAge() + 8
              ).RandomEntry(SeedBasedRandomizer);
            }
          }

          if (ofExistingFamilyMember == null) {
            if (relationshipIsMaleable) {
              asRelationship = Enum.GetValues(typeof(Family.Relations)).Cast<Family.Relations>()
                .Except(new[] { Family.Relations.Child })
                .RandomEntry(SeedBasedRandomizer);
            }
            else {
              throw new ArgumentException($"Cannot have a child older than their parent.");
            }
          }

          if (asRelationship == Relations.Parent) {
            if (newFamilyMember.GetAge() > ofExistingFamilyMember.GetAge()) {
              ofExistingFamilyMember = Members.Where(fm =>
                fm.GetAge() > newFamilyMember.GetAge() + 8
              ).RandomEntry(SeedBasedRandomizer);
            }

            if (ofExistingFamilyMember == null) {
              if (relationshipIsMaleable) {
                asRelationship = Enum.GetValues(typeof(Family.Relations)).Cast<Family.Relations>()
                  .Except(new[] { Family.Relations.Parent, Family.Relations.Child })
                  .RandomEntry(SeedBasedRandomizer);
                ofExistingFamilyMember = Members.RandomEntry(SeedBasedRandomizer);
              }
              else {
                throw new ArgumentException($"Cannot have a parent younger than their child, or a child older than their parent.");
              }
            }
          } else ofExistingFamilyMember = Members.RandomEntry(SeedBasedRandomizer);
        } else {
          if (asRelationship == Relations.Child) {
            if(newFamilyMember.GetAge() > ofExistingFamilyMember.GetAge()) {
              throw new ArgumentException($"Cannot have a child older than their parent.");
            }
          }
          if (asRelationship == Relations.Parent) {
            if (newFamilyMember.GetAge() > ofExistingFamilyMember.GetAge()) {
              throw new ArgumentException($"Cannot have a parent younger than their child.");
            }
          }
        }

        // add the root and reverse relationships:
        _relations.Add(newFamilyMember, new() {
          { asRelationship.Value.GetPairedType(), new() { ofExistingFamilyMember } }
        });
        _addRelationshipToExistingFamilyMember(newFamilyMember, asRelationship.Value, ofExistingFamilyMember);

        // make all the tree connections:
        Relationship newRelationship = new(newFamilyMember, asRelationship.Value, ofExistingFamilyMember);
        _addOtherDirectRelationships(newRelationship);

        newFamilyMember._addToFamily(this);
        return newRelationship;
      }

      void _addOtherDirectRelationships(Relationship newRelationship) {
        // if this is a new child of something
        switch (newRelationship.Relation) {
          case Relations.Child:
            // add siblings
            foreach (Relationship parentSiblingRelationship in GetDirectRelationships(newRelationship.Relative, Relations.Child)) {
              if (parentSiblingRelationship.CurrentEntity.Id != newRelationship.CurrentEntity.Id) {
                Entity currentEntity = newRelationship.CurrentEntity;
                Entity sibling = parentSiblingRelationship.Relative;
                _addRelationshipToBothExistingFamilyMembers(currentEntity, Relations.Sibling, sibling);
              }
            }
            // add other parents
            foreach (Relationship spousalRelationshipToParent in GetDirectRelationships(newRelationship.Relative, Relations.Spouse)) {
              Entity currentEntity = newRelationship.CurrentEntity;
              Entity otherParent = spousalRelationshipToParent.Relative;
              _addRelationshipToBothExistingFamilyMembers(currentEntity, Relations.Child, otherParent);
            }

            break;
          // if this is a new parent of something.
          case Relations.Sibling:
            // add parents and siblings shared by the current sibling
            HashSet<string> existingSiblings = new() { newRelationship.Relative.Id };
            HashSet<string> existingParents = new();
            foreach (Relationship siblingRelationship in GetDirectRelationships(newRelationship.Relative)) {
              Entity currentEntity = newRelationship.CurrentEntity;
              if (siblingRelationship.Relation == Relations.Sibling && !existingSiblings.Contains(siblingRelationship.Relative.Id)) {
                Entity sibling = siblingRelationship.Relative;
                _addRelationshipToBothExistingFamilyMembers(currentEntity, Relations.Sibling, sibling);
                existingSiblings.Add(siblingRelationship.Relative.Id);
              }
              else if (siblingRelationship.Relation == Relations.Parent && !existingParents.Contains(siblingRelationship.Relative.Id)) {
                Entity parent = siblingRelationship.Relative;
                _addRelationshipToBothExistingFamilyMembers(currentEntity, Relations.Child, parent);
                existingParents.Add(siblingRelationship.Relative.Id);
              }
            }

            break;
          case Relations.Spouse:
          case Relations.Parent:
            break;
        }
      }

      /// <summary>
      /// Adds the relationship back and forth.
      /// </summary>
      void _addRelationshipToBothExistingFamilyMembers(Entity forwardCurrentEntity, Relations forwardRelationshipType, Entity forwardRelatedEntity) {
        _addRelationshipToExistingFamilyMember(forwardCurrentEntity, forwardRelationshipType, forwardRelatedEntity);
        _addRelationshipToExistingFamilyMember(forwardRelatedEntity, forwardRelationshipType.GetPairedType(), forwardCurrentEntity);
      }

      void _addRelationshipToExistingFamilyMember(Entity newFamilyMember, Relations asRelationship, Entity withExistingFamilyMember) {
        if (asRelationship is Relations.Spouse) {
          _couples.Add(newFamilyMember, withExistingFamilyMember);
        }

        if (_relations[withExistingFamilyMember].TryGetValue(asRelationship, out var existingRelationshipsOfThisType)) {
          existingRelationshipsOfThisType.Add(newFamilyMember);
        }
        else {
          _relations[withExistingFamilyMember].Add(asRelationship, new() { newFamilyMember } );
        }
      }
    }
  }

  public static class RelationshipExtensions {

    /// <summary>
    /// Get the paired other half of the given relationship type.
    /// </summary>
    public static Entity.Family.Relations GetPairedType(this Entity.Family.Relations relationshipType) 
      => relationshipType switch {
        Entity.Family.Relations.Parent => Entity.Family.Relations.Child,
        Entity.Family.Relations.Child => Entity.Family.Relations.Parent,
        Entity.Family.Relations.Sibling => Entity.Family.Relations.Sibling,
        Entity.Family.Relations.Spouse => Entity.Family.Relations.Spouse,
        _ => throw new ArgumentException($"Unrecognized relationship type: {relationshipType}"),
      };
  }
}