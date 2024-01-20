using Kitchen;
using KitchenMods;
using Pets.Components;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems
{
    [UpdateInGroup(typeof(PostTransitionGroup))]
    public class TeleportPetsToOwners : RestaurantSystem, IModSystem
    {
        private EntityQuery _pets;
        protected override void Initialise()
        {
            base.Initialise();
            RequireSingletonForUpdate<CSceneFirstFrame>();
            _pets = GetEntityQuery(typeof(CPet));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> pets = _pets.ToEntityArray(Allocator.Temp);
            
            foreach (Entity pet in pets)
            {
                if (!Require(pet, out CPet cPet)) continue;
                if (cPet.Owner == Entity.Null) continue;
                if (!Require(cPet.Owner, out CPosition cPosition)) continue;
                if (!Require(pet, out CPosition cPetPosition)) continue;
                
                cPetPosition = new CPosition
                {
                    Position = cPosition.Position,
                    ForceSnap = true
                };
                EntityManager.SetComponentData(pet, cPetPosition);
            }
        }
    }
}