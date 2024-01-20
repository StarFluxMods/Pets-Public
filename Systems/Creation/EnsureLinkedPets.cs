using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Creation;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems
{
    [UpdateBefore(typeof(CleanPetlessPlayers))]
    public class EnsureLinkedPets : GameSystemBase, IModSystem
    {
        private EntityQuery _pets;
        private EntityQuery _playersWithLink;
        
        
        protected override void Initialise()
        {
            base.Initialise();
            _pets = GetEntityQuery(new QueryHelper().All(typeof(CPet)));
            _playersWithLink = GetEntityQuery(new QueryHelper().All(typeof(CLinkedPet)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> pets = _pets.ToEntityArray(Allocator.Temp);
            NativeArray<Entity> playersWithLink = _playersWithLink.ToEntityArray(Allocator.Temp);

            foreach (Entity player in playersWithLink)
            {
                if (!Require(player, out CLinkedPet cLinkedPet)) continue;
                if ((cLinkedPet.PetEntity == Entity.Null || cLinkedPet.PetEntity.Index == 0) || (cLinkedPet.PetEntity != Entity.Null && !EntityManager.HasComponent<CPet>(cLinkedPet.PetEntity)))
                {
                    foreach (Entity pet in pets)
                    {
                        if (!Require(pet, out CPet cPet)) continue;
                        if (cLinkedPet.PetLinkId != cPet.PetLinkId) continue;
                        cLinkedPet.PetEntity = pet;
                        cPet.Owner = player;
                        EntityManager.SetComponentData(player, cLinkedPet);
                        EntityManager.SetComponentData(pet, cPet);
                        break;
                    }
                }
            }

            foreach (Entity pet in pets)
            {
                if (!Require(pet, out CPet cPet)) continue;
                if ((cPet.Owner == Entity.Null || cPet.Owner.Index == 0) || (cPet.Owner != Entity.Null && !EntityManager.HasComponent<CLinkedPet>(cPet.Owner)))
                {
                    foreach (Entity player in playersWithLink)
                    {
                        if (!Require(player, out CLinkedPet cLinkedPet)) continue;
                        if (cLinkedPet.PetLinkId != cPet.PetLinkId) continue;
                        cLinkedPet.PetEntity = pet;
                        cPet.Owner = player;
                        EntityManager.SetComponentData(player, cLinkedPet);
                        EntityManager.SetComponentData(pet, cPet);
                        break;
                    }
                }
            }

            pets.Dispose();
            playersWithLink.Dispose();
        }
    }
}