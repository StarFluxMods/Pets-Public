using Kitchen;
using KitchenMods;
using Pets.Components.Creation;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems
{
    public class PetSelector : GameSystemBase, IModSystem
    {
        public static PetSelector Main;
        
        private EntityQuery _players;
        private EntityQuery _requestedPets;
        
        protected override void Initialise()
        {
            base.Initialise();
            Main = this;
            _players = GetEntityQuery(new QueryHelper().All(typeof(CPlayer)));
            _requestedPets = GetEntityQuery(new QueryHelper().All(typeof(CRequestedPet)));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> requestedPets = _requestedPets.ToEntityArray(Allocator.Temp); 
            using NativeArray<Entity> players = _players.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < requestedPets.Length; i++)
            {
                Entity requestedPet = requestedPets[i];
                if (!Require(requestedPet, out CRequestedPet cRequestedPet)) continue;
                foreach (Entity player in players)
                {
                    if (!Require(player, out CPlayer cPlayer)) continue;
                    if (cPlayer.ID != cRequestedPet.player) continue;
                    EntityManager.AddComponentData(player, new CRequiresPet
                    {
                        PetType = cRequestedPet.pet
                    });
                    EntityManager.DestroyEntity(requestedPet);
                }
            }
        }

        public void CreatePlayerRequest(int player, int pet)
        {
            Entity entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new CRequestedPet
            {
                pet = pet,
                player = player
            });
        }
    }
}