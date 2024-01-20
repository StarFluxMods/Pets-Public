using System.Collections.Generic;
using Kitchen;
using KitchenMods;
using Pets.Components.Properties;
using Pets.Components.Status;
using Pets.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pets.Systems.Activities
{
    public class PetEatActivity : PetActivitySystem, IModSystem
    {
        protected override PetState StateForUpdate => PetState.Eat;
        
        private EntityQuery _food;
        
        protected override void Initialise()
        {
            base.Initialise();
            _food = GetEntityQuery(new QueryHelper().All(typeof(CItemProvider), typeof(CPosition)).None(typeof(COccupiedByPet)));
        }

        protected override bool IsPossible(ActivityData data)
        {
            if (_food.CalculateEntityCount() == 0) return false;
            
            using NativeArray<Entity> food = _food.ToEntityArray(Allocator.Temp);

            Dictionary<float, Entity> ValidFood = GetAllEntitiesInRange(food, data.PetPosition);

            bool foundFood = false;
            
            foreach (float distance in ValidFood.Keys)
            {
                Entity foodEntity = ValidFood[distance];

                float offset = 0;

                if (Require(data.Pet, out CStandBackFromFood cStandBackFromFood))
                    offset = cStandBackFromFood.Distance;
                
                if (Require(data.Pet, out CPreferredFoods cPreferredFoods))
                {
                    if (Require(foodEntity, out CItemProvider cItemProvider))
                    {
                        if (!cPreferredFoods.PreferredFoods.Contains(cItemProvider.ProvidedItem)) continue;
                    }
                }
                
                if (!Require(foodEntity, out CPosition cFoodPosition)) continue;
                
                Vector3 targetPos = Vector3.zero;
                
                if (!CanGetTo(data.PetPosition, cFoodPosition, new Vector3(1 + offset, 0, 0), out targetPos))
                    if (!CanGetTo(data.PetPosition, cFoodPosition, new Vector3(-1 - offset, 0, 0), out targetPos))
                        if (!CanGetTo(data.PetPosition, cFoodPosition, new Vector3(0, 0, 1 + offset), out targetPos))
                            if (!CanGetTo(data.PetPosition, cFoodPosition, new Vector3(0, 0, -1 - offset), out targetPos))
                                continue;

                foundFood = true;
                TargetPosition = targetPos;
                TargetEntity = foodEntity;
                TargetForward = cFoodPosition;
                break;
            }

            return foundFood;
        }

        private Entity TargetEntity;
        private CPosition TargetPosition;
        private CPosition TargetForward;
        
        protected override bool Perform(ActivityData data)
        {
            if (!Require(TargetEntity, out CPosition cBedPosition)) return false;
            
            EntityManager.AddComponentData(data.Pet, new CMoveToLocation
            {
                Location = TargetPosition,
                StoppingDistance = 0,
                DesiredFacing = TargetForward
            });

            EntityManager.AddComponentData(TargetEntity, new COccupiedByPet());
            EntityManager.AddComponentData(data.Pet, new CPetInteractingWith
            {
                InteractingWith = TargetEntity,
                TimeToFinish = Random.Range(5, 15),
                IsWaitingForDestination = true
            });

            return HasComponent<CPetInteractingWith>(data.Pet) && HasComponent<CMoveToLocation>(data.Pet) && HasComponent<COccupiedByPet>(TargetEntity);
        }
    }
}