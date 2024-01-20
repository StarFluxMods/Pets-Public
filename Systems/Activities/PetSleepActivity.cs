using System;
using System.Collections.Generic;
using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Properties;
using Pets.Components.Status;
using Pets.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pets.Systems.Activities
{
    public class PetSleepActivity : PetActivitySystem, IModSystem
    {
        protected override PetState StateForUpdate => PetState.Sleep;
        
        private EntityQuery _petBeds;
        
        protected override void Initialise()
        {
            base.Initialise();
            _petBeds = GetEntityQuery(new QueryHelper().All(typeof(CPetBed), typeof(CPosition)).None(typeof(COccupiedByPet)));
        }

        protected override bool IsPossible(ActivityData data)
        {
            if (_petBeds.CalculateEntityCount() == 0) return false;
            
            using NativeArray<Entity> petBeds = _petBeds.ToEntityArray(Allocator.Temp);

            Dictionary<float, Entity> ValidBeds = GetAllEntitiesInRange(petBeds, data.PetPosition);

            bool foundBed = false;
            
            foreach (float distance in ValidBeds.Keys)
            {
                Entity bed = ValidBeds[distance];
                CPosition bedPosition;
                if (!Require(bed, out bedPosition)) continue;
                if (Require(data.Pet, out CSleepingPositionOffset cSleepingPositionOffset))
                    if (GetOffsetPosition(bedPosition.Forward(1), cSleepingPositionOffset.Offset, out Vector2 offsetPosition))
                        bedPosition += offsetPosition;
                    
                
                
                if (!CanGetTo(data.PetPosition, bedPosition, Vector3.zero, out Vector3 targetDestination)) continue;
                
                foundBed = true;
                TargetPosition = targetDestination;
                TargetEntity = bed;
                break;
            }

            return foundBed;
        }

        private Entity TargetEntity;
        private CPosition TargetPosition;
        
        protected override bool Perform(ActivityData data)
        {
            if (!Require(TargetEntity, out CPosition cBedPosition)) return false;
            
            EntityManager.AddComponentData(data.Pet, new CMoveToLocation
            {
                Location = TargetPosition,
                StoppingDistance = 0,
                DesiredFacing = cBedPosition.Forward(-10)
            });

            EntityManager.AddComponentData(TargetEntity, new COccupiedByPet());
            EntityManager.AddComponentData(data.Pet, new CPetInteractingWith
            {
                InteractingWith = TargetEntity,
                StartTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                TimeToFinish = Random.Range(5, 15),
                IsWaitingForDestination = true
            });

            return HasComponent<CPetInteractingWith>(data.Pet) && HasComponent<CMoveToLocation>(data.Pet) && HasComponent<COccupiedByPet>(TargetEntity);
        }
    }
}