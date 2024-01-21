using System;
using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Status;
using Pets.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Pets.Systems.Activities
{
    public class StopInteraction : GameSystemBase, IModSystem
    {
        private EntityQuery _pets;
        
        protected override void Initialise()
        {
            base.Initialise();
            _pets = GetEntityQuery(new QueryHelper().All(typeof(CPet), typeof(CPetInteractingWith)));
        }

        protected override void OnUpdate()
        {
            if (_pets.CalculateEntityCount() == 0) return;
            
            NativeArray<Entity> pets = _pets.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < pets.Length; i++)
            {
                Entity pet = pets[i];
                if (!Require(pet, out CPetInteractingWith cPetInteractingWith)) continue;
                if (!Require(pet, out CMoveToLocation cMoveToLocation)) continue;
                if (!Require(pet, out CPosition cPosition)) continue;
                if (!Require(pet, out CCurrentSpeed cCurrentSpeed)) continue;
                if (cPetInteractingWith.InteractingWith == Entity.Null)
                {
                    EntityManager.RemoveComponent<CPetInteractingWith>(pet);
                    continue;
                }

                if (cPetInteractingWith.IsWaitingForDestination)
                {
                    if (Vector3.Distance(cPosition, cMoveToLocation.Location) < 0.1f && cCurrentSpeed.speed < Mod.MinimumSpeedThreshold)
                    {
                        cPetInteractingWith.IsWaitingForDestination = false;
                        cPetInteractingWith.StartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                        EntityManager.AddComponentData(pet, cPetInteractingWith);
                    }
                    continue;
                }

                if (!Require(pet, out CPet cPet)) continue;
                if (cPetInteractingWith.StartTime + cPetInteractingWith.TimeToFinish > DateTimeOffset.Now.ToUnixTimeSeconds()) continue;
                
                EntityManager.RemoveComponent<CMoveToLocation>(pet);
                EntityManager.RemoveComponent<COccupiedByPet>(cPetInteractingWith.InteractingWith);
                EntityManager.RemoveComponent<CPetInteractingWith>(pet);
                cPet.State = PetState.Follow;
                EntityManager.AddComponentData(pet, cPet);
                
            }

            pets.Dispose();
        }
    }
}