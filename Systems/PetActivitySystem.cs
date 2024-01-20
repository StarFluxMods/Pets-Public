using System.Collections.Generic;
using System.Linq;
using Kitchen;
using KitchenData;
using Pets.Components;
using Pets.Components.Status;
using Pets.Customs.Types;
using Pets.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

namespace Pets.Systems
{
    public abstract class PetActivitySystem : GameSystemBase
    {
        protected abstract bool IsPossible(ActivityData data);
        protected abstract bool Perform(ActivityData data);
        
        protected abstract PetState StateForUpdate { get; }

        private EntityQuery _petsWaitingForActivity;
        private NavMeshPath Path = new NavMeshPath();
        
        protected override void Initialise()
        {
            base.Initialise();
            _petsWaitingForActivity = GetEntityQuery(new QueryHelper().All(typeof(CPet)).None(typeof(CPetInteractingWith)));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> petsWaitingForActivity = _petsWaitingForActivity.ToEntityArray(Allocator.Temp);
            petsWaitingForActivity.ShuffleInPlace();
            
            if (petsWaitingForActivity.Length == 0) return;
            
            Entity pet = petsWaitingForActivity[0];
            
            if (!Require(pet, out CPet cPet)) return;
            if (!GameData.Main.TryGet(cPet.PetType, out Pet petType)) return;
            if (!Require(pet, out CPosition cPosition)) return;
            if (!Require(pet, out CDefaultState cDefaultState)) return;
            if (cPet.State != StateForUpdate) return;
            
            ActivityData data = new ActivityData
            {
                Pet = pet,
                PetPosition = cPosition,
                PetType = petType,
                State = cPet.State
            };

            
            if (IsPossible(data))
            {
                if (Perform(data))
                {
                    return;
                }
            }
            
            
            cPet.State = cDefaultState.State;
            EntityManager.SetComponentData(pet, cPet);
        }

        protected Dictionary<float, Entity> GetAllEntitiesInRange(NativeArray<Entity> entities, Vector3 startingPosition, float maxDistance = float.MaxValue, bool sortByDistance = true)
        {
            Dictionary<float, Entity> entitiesInRange = new();

            foreach (Entity entity in entities)
            {
                if (!Require(entity, out CPosition cPosition)) continue;
                float distance = Vector3.Distance(startingPosition, cPosition);
                if (distance > maxDistance) continue;
                if (entitiesInRange.ContainsKey(distance)) continue;
                entitiesInRange.Add(distance, entity);
            }
            
            if (sortByDistance)
                entitiesInRange = entitiesInRange.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            return entitiesInRange;
        }
        
        protected bool CanGetTo(Vector3 startingPosition, Vector3 targetPosition, Vector3 targetOffset, out Vector3 targetDestination)
        {
            targetDestination = Vector3.zero;
            CLayoutRoomTile tile1 = GetTile(targetPosition);
            CLayoutRoomTile tile2 = GetTile(targetPosition + targetOffset);
            if (tile1.RoomID != tile2.RoomID) return false;
            
            NavMesh.CalculatePath(startingPosition, targetPosition + targetOffset, NavMesh.AllAreas, Path);
            
            if (Path.status == NavMeshPathStatus.PathComplete)
                targetDestination = targetPosition + targetOffset;
            
            return Path.status == NavMeshPathStatus.PathComplete;
        }
        
        protected bool GetOffsetPosition(Vector3 forward, Vector2 offset, out Vector2 success)
        {
            success = Vector2.zero;
            float rotation = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            Mod.Logger.LogInfo("GetOffsetPosition " + rotation);
            if (rotation == 0)
                success = offset;
            if (rotation == 90)
                success = new Vector2(offset.y, -offset.x);
            if (rotation == 180)
                success =  -offset;
            if (rotation == 270)
                success = new Vector2(-offset.y, offset.x);
            return success != Vector2.zero;
        }
    }

    public struct ActivityData
    {
        public Entity Pet;
        public CPosition PetPosition;
        public Pet PetType;
        public PetState State;
    }
}