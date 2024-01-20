using System.Collections.Generic;
using Kitchen;
using KitchenMods;
using Pets.Components.Creation;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Systems
{
    public class TransitionFix : GameSystemBase, IModSystem
    {
        private EntityQuery _players;
        private static Dictionary<int, CRequiresPet> _requiresPet = new Dictionary<int, CRequiresPet>();
        private static Dictionary<int, CLinkedPet> _linkedPet = new Dictionary<int, CLinkedPet>();
        
        protected override void Initialise()
        {
            base.Initialise();
            _players = GetEntityQuery(new QueryHelper().All(typeof(CPlayer)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> players = _players.ToEntityArray(Allocator.Temp);
            
            _requiresPet.Clear();
            _linkedPet.Clear();
            
            foreach (Entity player in players)
            {
                if (!Require(player, out CPlayer cPlayer)) continue;
                if (Require(player, out CRequiresPet cRequiresPet))
                {
                    _requiresPet.Add(cPlayer.ID, cRequiresPet);
                }
                if (Require(player, out CLinkedPet cLinkedPet))
                {
                    _linkedPet.Add(cPlayer.ID, cLinkedPet);
                }
            }

            players.Dispose();
        }

        public override void AfterLoading(SaveSystemType system_type)
        {
            base.AfterLoading(system_type);
            if (_requiresPet == null) return;
            
            NativeArray<Entity> players = _players.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < players.Length; i++)
            {
                Entity player = players[i];
                if (Has<CLinkedPet>(player))
                {
                    EntityManager.RemoveComponent<CLinkedPet>(player);
                }
                
                if (!Require(player, out CPlayer cPlayer)) continue;
                
                if (_linkedPet.TryGetValue(cPlayer.ID, out CLinkedPet cLinkedPet))
                {
                    EntityManager.AddComponentData(player, cLinkedPet);
                }

                if (_requiresPet.TryGetValue(cPlayer.ID, out CRequiresPet cRequiresPet))
                {
                    EntityManager.AddComponentData(player, cRequiresPet);
                }
            }

            _requiresPet.Clear();
            players.Dispose();
        }
    }
}