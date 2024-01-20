using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Properties;
using Pets.Components.Status;
using Pets.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Pets.Systems
{
    public class SelectActivity : GameSystemBase, IModSystem
    {
        private EntityQuery _pets;
        protected override void Initialise()
        {
            base.Initialise();
            _pets = GetEntityQuery(new QueryHelper().All(typeof(CPet)).None(typeof(CPetInteractingWith)));
        }
        
        protected override void OnUpdate()
        {
            NativeArray<Entity> pets = _pets.ToEntityArray(Allocator.Temp);

            foreach (Entity pet in pets)
            {
                if (!Require(pet, out CPet cPet)) continue;
                if (!Require(pet, out CDefaultState cDefaultState)) continue;
                if (cPet.State != cDefaultState.State) continue;
                if (!Require(pet, out CActivities cActivities)) continue;
                if (!(Random.value <= 0.03f * Time.DeltaTime)) continue;
                
                int activity = Random.Range(0, cActivities.Activities.Length);
                activity = cActivities.Activities[activity];

                cPet.State = (PetState)activity;
                EntityManager.SetComponentData(pet, cPet);
            }

            pets.Dispose();
        }
    }
}