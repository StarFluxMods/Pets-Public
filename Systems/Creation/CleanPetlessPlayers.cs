using Kitchen;
using KitchenMods;
using Pets.Components.Creation;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems
{
    [UpdateAfter(typeof(EnsureLinkedPets))]
    public class CleanPetlessPlayers : GameSystemBase, IModSystem
    {
        private EntityQuery _players;
        protected override void Initialise()
        {
            base.Initialise();
            _players = GetEntityQuery(new QueryHelper().All(typeof(CPlayer), typeof(CLinkedPet)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> players = _players.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < players.Length; i++)
            {
                Entity player = players[i];
                if (!Require(player, out CLinkedPet cLinkedPet)) continue;
                if (cLinkedPet.PetEntity != Entity.Null) continue;
                    
                Mod.Logger.LogInfo($"Removing petless player");
                
                EntityManager.RemoveComponent<CLinkedPet>(player);
            }

            players.Dispose();
        }
    }
}