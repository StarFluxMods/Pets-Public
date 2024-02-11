using System.Collections.Generic;
using KitchenLib.Utils;
using Pets.Components.Properties;
using Pets.Customs.Types;
using Pets.Enums;
using Pets.Interfaces;
using Unity.Collections;
using UnityEngine;

namespace Pets.Customs
{
    public class Panda : CustomPet
    {
        public override string UniqueNameID => "Panda";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Panda").AssignMaterialsByNames();
        public override GameObject IconPrefab => Mod.Bundle.LoadAsset<GameObject>("PandaIcon").AssignMaterialsByNames();
        public override PetState DefaultState => PetState.Follow;
        
        public override List<IPetProperty> Properties { get; protected set; } = new List<IPetProperty>()
        {
            new CActivities
            {
                Activities = new FixedListInt64
                {
                    (int)PetState.Follow,
                    (int)PetState.Eat,
                    (int)PetState.Sleep,
                }
            },
            new CSleepingPositionOffset
            {
                Offset = new Vector2(-0.785f, -0.36f)
            },
            new CRoamNearOwner(),
            new CStapleAppliances
            {
                Appliances = new FixedListInt64
                {
                    GDOUtils.GetCustomGameDataObject<PetBed>().ID
                }
            },
            new CStandBackFromFood
            {
                Distance = 0.5f
            }
        };
    }
}