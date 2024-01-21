using System;
using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Status;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Pets.Systems
{
    public class PetStuckCheck : GameSystemBase, IModSystem
    {
        private EntityQuery _pets;
        protected override void Initialise()
        {
            base.Initialise();
            _pets = GetEntityQuery(new QueryHelper().All(typeof(CPet)));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> pets = _pets.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < pets.Length; i++)
            {
                Entity pet = pets[i];
                if (!Require(pet, out CPet cPet)) continue;
                if (!Require(pet, out CPosition cPosition)) continue;
                if (!Require(pet, out CMoveToLocation cMoveToLocation)) continue;
                if (!Require(pet, out CCurrentSpeed cCurrentSpeed)) continue;

                if (Vector3.Distance(cMoveToLocation.Location, cPosition) > 0)
                {
                    if (Require(pet, out CPetStuckChecker cPetStuckChecker))
                    {
                        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                        if ((currentTime - cPetStuckChecker.LastCheck) < 5) continue;
                        if (!(Vector3.Distance(cPosition, cPetStuckChecker.LastPosition) < 0.1f))
                        {
                            EntityManager.AddComponentData(pet, new CPetStuckChecker
                            {
                                LastCheck = currentTime,
                                LastPosition = cPosition.Position
                            });
                        }
                        else
                        {
                            if (!Require(pet, out CDefaultState cDefaultState)) continue;
                            cPet.State = cDefaultState.State;
                            EntityManager.SetComponentData(pet, cPet);

                            if (Has<CPetStuckChecker>(pet))
                                EntityManager.RemoveComponent<CPetStuckChecker>(pet);

                            if (Has<CMoveToLocation>(pet))
                                EntityManager.RemoveComponent<CMoveToLocation>(pet);

                            if (Has<CCurrentSpeed>(pet))
                                EntityManager.AddComponentData(pet, new CCurrentSpeed
                                {
                                    speed = 0
                                });

                            if (Require(pet, out CPetInteractingWith cPetInteractingWith))
                            {
                                if (cPetInteractingWith.InteractingWith != Entity.Null)
                                {
                                    EntityManager.RemoveComponent<COccupiedByPet>(cPetInteractingWith.InteractingWith);
                                    EntityManager.RemoveComponent<CPetInteractingWith>(pet);
                                }
                            }

                            EntityManager.RemoveComponent<CMoveToLocation>(pet);
                        }
                    }
                    else
                    {
                        EntityManager.AddComponentData(pet, new CPetStuckChecker
                        {
                            LastCheck = DateTimeOffset.Now.ToUnixTimeSeconds(),
                            LastPosition = cPosition.Position
                        });
                    }
                }
                else
                {
                    if (Has<CPetStuckChecker>(pet))
                        EntityManager.RemoveComponent<CPetStuckChecker>(pet);
                }
            }
        }
    }
}