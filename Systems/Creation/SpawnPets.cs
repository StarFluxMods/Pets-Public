using Kitchen;
using KitchenData;
using KitchenMods;
using Pets.Components;
using Pets.Components.Creation;
using Pets.Customs.Types;
using Pets.Enums;
using Pets.Interfaces;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems.Creation
{
    [UpdateAfter(typeof(EnsureLinkedPets))]
    public class SpawnPets : GameSystemBase, IModSystem
    {
        private EntityQuery _players;
        
        protected override void Initialise()
        {
            base.Initialise();
            _players = GetEntityQuery(new QueryHelper().All(typeof(CPlayer), typeof(CRequiresPet)).None(typeof(CLinkedPet)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> players = _players.ToEntityArray(Allocator.Temp);

            foreach (Entity player in players)
            {
                if (!Require(player, out CRequiresPet cRequiresPet)) continue;
                if (!Require(player, out CPosition cPosition)) continue;
                if (!GameData.Main.TryGet(cRequiresPet.PetType, out Pet petType)) continue; 
                
                Entity pet = EntityManager.CreateEntity();
                
                int LinkID = UnityEngine.Random.Range(-100000, 100000);
                
                EntityManager.AddComponentData(pet, new CPet
                {
                    Owner = player,
                    State = petType.DefaultState,
                    PetLinkId = LinkID,
                    PetType = petType.ID
                });
                EntityManager.AddComponentData(pet, new CPosition(cPosition));
                
                EntityManager.AddComponentData(pet, new CRequiresView
                {
                    Type = petType.ViewType,
                    PhysicsDriven = true
                });
                EntityManager.AddComponentData(pet, new CDoNotPersist());
                EntityManager.AddComponentData(pet, new CIsInteractive());
                EntityManager.AddComponentData(pet, new CCanBePetted());
                EntityManager.AddComponentData(pet, new CPersistThroughSceneChanges());

                
                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
                foreach (IPetProperty property in petType.Properties)
                {
                    ecb.AddComponent(pet, (dynamic)property);
                }
                
                ecb.AddComponent(pet, new CDefaultState(petType.DefaultState));
                
                ecb.Playback(EntityManager);
                
                EntityManager.AddComponentData(player, new CLinkedPet
                {
                    PetType = cRequiresPet.PetType,
                    PetEntity = pet,
                    PetLinkId = LinkID
                });
                
            }

            players.Dispose();
        }
    }
}