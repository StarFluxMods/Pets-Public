using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Properties;
using Pets.Components.Status;
using Pets.Enums;
using Unity.Entities;
using UnityEngine;

namespace Pets.Systems.Activities
{
    public class PetFollowActivity : PetActivitySystem, IModSystem
    {
        protected override PetState StateForUpdate => PetState.Follow;

        protected override bool IsPossible(ActivityData data)
        {
            maxDistance = 5;
            
            if (!Require(data.Pet, out CPet cPet)) return false;
            if (cPet.Owner == Entity.Null) return false;
            if (!Require(cPet.Owner, out TargetPosition)) return false;
            if (Require(data.Pet, out CLonelyDistance cLonelyDistance))
                maxDistance = cLonelyDistance.Distance;
            
            currentDistance = Vector3.Distance(data.PetPosition, TargetPosition);
            
            return true;
        }

        private float maxDistance;
        private float currentDistance;
        private CPosition TargetPosition;
        
        protected override bool Perform(ActivityData data)
        {
            if (currentDistance > maxDistance)
            {
                EntityManager.AddComponentData(data.Pet, new CMoveToLocation
                {
                    Location = TargetPosition,
                    StoppingDistance = 1,
                });
            }
            else
            {
                if (!Require(data.Pet, out CRoamNearOwner cRoamNearOwner)) return true;
                
                if (!(Random.value <= 0.03f * Time.DeltaTime)) return true;
                
                if (!Require(data.Pet, out CCurrentSpeed cCurrentSpeed)) return true;
                if (cCurrentSpeed.speed > Mod.MinimumSpeedThreshold) return true;
                
                Vector3 randomPosition = new Vector3(Random.Range(-cRoamNearOwner.Distance, cRoamNearOwner.Distance), 0, Random.Range(-cRoamNearOwner.Distance, cRoamNearOwner.Distance));
                Vector3 newTarget = TargetPosition + randomPosition;
                float distance = Vector3.Distance(data.PetPosition, newTarget);
                
                if (!(distance <= maxDistance)) return true;
                if (!CanGetTo(data.PetPosition, newTarget, Vector3.zero, out newTarget)) return true;
                
                EntityManager.AddComponentData(data.Pet, new CMoveToLocation
                {
                    Location = newTarget,
                    StoppingDistance = 0
                });
                
                EntityManager.AddComponentData(data.Pet, new CPetInteractingWith
                {
                    TimeToFinish = 0,
                    IsWaitingForDestination = true
                });
            }

            return true;
        }
    }
}