using Kitchen;
using KitchenMods;
using Pets.Components.Creation;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems
{
    public class EnsureCorrectPet : GameSystemBase, IModSystem
    {
        private EntityQuery _players;
        
        protected override void Initialise()
        {
            base.Initialise();
            _players = GetEntityQuery(typeof(CPlayer), typeof(CLinkedPet), typeof(CRequiresPet));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> players = _players.ToEntityArray(Allocator.Temp);
            
            for (int i = 0; i < players.Length; i++)
            {
                Entity player = players[i];
                if (!Require(player, out CLinkedPet cLinkedPet)) continue;
                if (!Require(player, out CRequiresPet cRequiresPet)) continue;

                if (cLinkedPet.PetType != cRequiresPet.PetType && cLinkedPet.PetEntity != Entity.Null)
                {
                    EntityManager.DestroyEntity(cLinkedPet.PetEntity);
                    EntityManager.RemoveComponent<CLinkedPet>(player);
                }
            }
            
            players.Dispose();
        }
    }
}