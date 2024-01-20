using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Creation;
using Pets.Enums;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems
{
    public class FillStateChangeRequests : GameSystemBase, IModSystem
    {
        private EntityQuery _players;
        private EntityQuery _stateChangeRequests;
        protected override void Initialise()
        {
            base.Initialise();
            _players = GetEntityQuery(typeof(CPlayer));
            _stateChangeRequests = GetEntityQuery(typeof(CRequestStateChange));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> stateChangeRequests = _stateChangeRequests.ToEntityArray(Allocator.Temp);
            using NativeArray<Entity> players = _players.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < stateChangeRequests.Length; i++)
            {
                Entity request = stateChangeRequests[i];
                if (!Require(request, out CRequestStateChange cRequestStateChange)) continue;

                foreach (Entity player in players)
                {
                    if (!Require(player, out CPlayer cPlayer)) continue;
                    if (cPlayer.ID != cRequestStateChange.PlayerID) continue;
                    if (!Require(player, out CLinkedPet cLinkedPet)) continue;
                    if (!Require(cLinkedPet.PetEntity, out CPet cPet)) continue;
                    cPet.State = (PetState)cRequestStateChange.StateID;
                    EntityManager.AddComponentData(cLinkedPet.PetEntity, cPet);
                    EntityManager.DestroyEntity(request);
                }
            }
        }
    }
}