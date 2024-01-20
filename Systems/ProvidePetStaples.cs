using System.Collections.Generic;
using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using KitchenLib.Utils;
using KitchenMods;
using Pets.Components.Properties;
using Pets.Customs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Pets.Systems
{
    public class ProvidePetStaples : StartOfNightSystem, IModSystem
    {
        private EntityQuery _staplePetAppliances;
        protected override void Initialise()
        {
            base.Initialise();
            _staplePetAppliances = GetEntityQuery(typeof(CStapleAppliances));
        }
        
        protected override void OnUpdate()
        {
            
            using NativeArray<Entity> staplePetAppliances = _staplePetAppliances.ToEntityArray(Allocator.Temp);
            
            foreach (Entity staplePetAppliance in staplePetAppliances)
            {
                if (!Require(staplePetAppliance, out CStapleAppliances cStapleAppliances)) continue;
                
                foreach (int applianceID in cStapleAppliances.Appliances)
                {
                    if (!(Random.value <= 0.3f * Time.DeltaTime)) continue;
                        
                    List<Vector3> postTiles = GetPostTiles();
                    int num = 0;
                    Vector3 position = FindTile(ref num, postTiles);
                    CreateBlueprintLetter(EntityManager, position, applianceID, 0, 0, true);
                }
            }
        }

        public Vector3 FindTile(ref int placed_tile, List<Vector3> floor_tiles)
        {
            Vector3 vector = default;
            bool flag = false;
            while (!flag && placed_tile < floor_tiles.Count)
            {
                int num = placed_tile;
                placed_tile = num + 1;
                vector = floor_tiles[num];
                if (GetOccupant(vector) != default) continue;
                flag = true;
                foreach (LayoutPosition pos in LayoutHelpers.Directions)
                {
                    Entity occupant = GetOccupant(new Vector3(vector.x + pos.x, vector.y, vector.z + pos.y));
                    if (occupant == default || !Has<CApplianceTable>(occupant)) continue;
                    flag = false;
                    break;
                }
            }

            return !flag ? GetFallbackTile() : vector;
        }
        
        public static Entity CreateBlueprintLetter(EntityManager em, Vector3 position, int appliance_id, float price_multiplier = 1f, int force_price = -1, bool use_red = false)
        {
            Entity entity = em.CreateEntity();
            em.AddComponentData<CCreateAppliance>(entity, new CCreateAppliance
            {
                ID = GDOUtils.GetCustomGameDataObject<PetLetter>().ID
            });
            em.AddComponentData<CPosition>(entity, new CPosition(position));
            em.AddComponentData<CLetter>(entity, default(CLetter));
            int num = 0;
            Appliance appliance;
            if (force_price >= 0)
            {
                num = force_price;
            }
            else if (GameData.Main.TryGet<Appliance>(appliance_id, out appliance, true))
            {
                num = appliance.PurchaseCost;
            }
            em.AddComponentData<CLetterBlueprint>(entity, new CLetterBlueprint
            {
                BlueprintID = AssetReference.Blueprint,
                ApplianceID = appliance_id,
                Price = Mathf.CeilToInt((float)num * price_multiplier)
            });
            em.AddComponentData<CShopEntity>(entity, default(CShopEntity));
            return entity;
        }
    }
}