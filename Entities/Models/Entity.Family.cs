using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {

  public partial class Entity {

    /// <summary>
    /// A family made of entities.
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

        if (routeSpouse != null) {
          AddFamilyMember(routeSpouse, Relations.Spouse, RootMember);
        }
      }

      /// <summary>
      /// Add a new member to a family.
      /// (optional) you can provide a relationship with an existing family member already, if not, one will be generated.
      /// </summary>
      public virtual Relationship AddFamilyMember(Entity newFamilyMember, Relations? asRelationship = null, Entity ofExistingFamilyMember = null) {
        if (asRelationship is null) {
          asRelationship = Enum.GetValues(typeof(Family.Relations)).Cast<Family.Relations>().RandomEntry(SeedBasedRandomizer);
        }
        if (ofExistingFamilyMember is null) {
          ofExistingFamilyMember = Members.RandomEntry(SeedBasedRandomizer);
        }

        // add the root and reverse relationship:
        _relations.Add(newFamilyMember, new() {
          { asRelationship.Value.GetPairedType(), new() { ofExistingFamilyMember } }
        });
        _addRelationshipToExistingFamilyMember(newFamilyMember, asRelationship.Value, ofExistingFamilyMember);

        // make all the tree connections:
        Relationship newRelationship = new(newFamilyMember, asRelationship.Value, ofExistingFamilyMember);
        _addOtherDirectRelationships(newRelationship);

        return newRelationship;
      }

      void _addOtherDirectRelationships(Relationship newRelationship) {
        // if this is a new child of something
        if (newRelationship.Relation == Relations.Child) {
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
        } // if this is a new parent of something.
        else if (newRelationship.Relation == Relations.Parent) {
          // no other connections needed
        }// if this is the sibling of something
        else if (newRelationship.Relation == Relations.Sibling) {
          // add parents and siblings shared by the current sibling
          HashSet<string> existingSiblings = new() { newRelationship.Relative.Id };
          HashSet<string> existingParents = new();
          foreach (Relationship siblingRelationship in GetDirectRelationships(newRelationship.Relative)) {
            Entity currentEntity = newRelationship.CurrentEntity;
            if (siblingRelationship.Relation == Relations.Sibling && !existingSiblings.Contains(siblingRelationship.Relative.Id)) {
              Entity sibling = siblingRelationship.Relative;
              _addRelationshipToBothExistingFamilyMembers(currentEntity, Relations.Sibling, sibling);
              existingSiblings.Add(siblingRelationship.Relative.Id);
            } else if (siblingRelationship.Relation == Relations.Parent && !existingParents.Contains(siblingRelationship.Relative.Id)) {
              Entity parent = siblingRelationship.Relative;
              _addRelationshipToBothExistingFamilyMembers(currentEntity, Relations.Child, parent);
              existingParents.Add(siblingRelationship.Relative.Id);
            }
          }
          // add other siblings
          foreach (Relationship parentSiblingRelationship in GetDirectRelationships(newRelationship.Relative, Relations.Child)) {
            if (parentSiblingRelationship.CurrentEntity.Id != newRelationship.CurrentEntity.Id) {
              Entity currentEntity = newRelationship.CurrentEntity;
              Entity sibling = parentSiblingRelationship.Relative;
              _addRelationshipToBothExistingFamilyMembers(currentEntity, Relations.Sibling, sibling);
            }
          }
        }
      }

      /// <summary>
      /// Adds the relationship back and forth.
      /// </summary>
      void _addRelationshipToBothExistingFamilyMembers(Entity forwardCurrentEntity, Relations forwardRelationshipType, Entity forwardRelatedEntity) {
        _addRelationshipToExistingFamilyMember(forwardCurrentEntity, forwardRelationshipType, forwardRelatedEntity);
        _addRelationshipToExistingFamilyMember(forwardRelatedEntity, forwardRelationshipType.GetPairedType(), forwardCurrentEntity);
      }

      /*void _recursivelyConnectAllOtherRelationships(Relationship relationship, HashSet<string> alreadyCalculatedRelatives = null) {
        alreadyCalculatedRelatives ??= new() { relationship.Relative.Id};
        foreach (Relationship adjacentRelationship in GetDirectRelationships(relationship.Relative).Where(r => !alreadyCalculatedRelatives.Contains(r.CurrentEntity.Id))) {
          alreadyCalculatedRelatives.Add(adjacentRelationship.Relative.Id);
          Relationship newRelationship = _triangulateNewRelationship(relationship, adjacentRelationship);
          _addRelationshipToExistingFamilyMember(newRelationship.Relative, newRelationship.Relation.GetPairedType(), newRelationship.CurrentEntity, newRelationship.ExtraFamilialDistance);
          _addRelationshipToExistingFamilyMember(newRelationship.CurrentEntity, newRelationship.Relation, newRelationship.Relative, newRelationship.ExtraFamilialDistance);
          _recursivelyConnectAllOtherRelationships(newRelationship, alreadyCalculatedRelatives);
        }
      }

      Relationship _triangulateNewRelationship(Relationship relationshipWithNewCurrentEntity, Relationship adjacentRelationshipWithExistingRelatedEntity) {
        if (relationshipWithNewCurrentEntity.Relative.Id != adjacentRelationshipWithExistingRelatedEntity.CurrentEntity.Id) {
          throw new ArgumentException($"The intermediate entity is not the same for the given relationships. Cannot triangulate a new relationship between them");
        }

        Entity currentEntity = relationshipWithNewCurrentEntity.CurrentEntity;
        Entity relatedEntity = relationshipWithNewCurrentEntity.Relative;
        Relations newEntitysRelationshipWithSharedEntity = relationshipWithNewCurrentEntity.Relation;
        Relations sharedEntitysRelationshipWithExistingEntity = adjacentRelationshipWithExistingRelatedEntity.Relation;
        Relations newEntitysRelationshipWithExistingEntity;
        int extraFamilyDistance;

        // if the shared entity is the x of the new related entity.
        switch (sharedEntitysRelationshipWithExistingEntity) {
          case Relations.Parent:
            // if the new entity is the y of the x of the existing entity.
            switch (newEntitysRelationshipWithSharedEntity) {
              case Relations.Spouse:
                newEntitysRelationshipWithExistingEntity = Relations.Parent;
                extraFamilyDistance = relationshipWithNewCurrentEntity.ExtraFamilialDistance;
                break;
              case Relations.Cousin:
                newEntitysRelationshipWithExistingEntity = Relations.Pibling;
                extraFamilyDistance = relationshipWithNewCurrentEntity.ExtraFamilialDistance + 1;
                break;
              case Relations.Parent:
                newEntitysRelationshipWithExistingEntity = Relations.Parent;
                extraFamilyDistance = relationshipWithNewCurrentEntity.ExtraFamilialDistance + 1;
                break;
              case Relations.Child:
                break;
              case Relations.Sibling:
                break;
              case Relations.Newphew:
                break;
              case Relations.Pibling:
                break;
            }
            break;
          case Relations.Cousin:
            break;
          case Relations.Child:
            break;
          case Relations.Sibling:
            break;
          case Relations.Newphew:
            break;
          case Relations.Pibling:
            break;
          case Relations.Spouse:
            break;
        }

        return new Relationship(newEntitysRelationshipWithExistingEntity, extraFamilyDistance, currentEntity, relatedEntity);
      }*/

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