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
    public class Penguin : CustomPet
    {
        public override string UniqueNameID => "Penguin";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Penguin").AssignMaterialsByNames();
        public override GameObject IconPrefab => Mod.Bundle.LoadAsset<GameObject>("PenguinIcon").AssignMaterialsByNames();
        public override PetState DefaultState => PetState.Follow;
        
        public override List<IPetProperty> Properties { get; protected set; } = new List<IPetProperty>()
        {
            new CActivities
            {
                Activities = new FixedListInt64
                {
                    (int)PetState.Follow,
                    (int)PetState.Sleep,
                }
            },
            new CSleepingPositionOffset
            {
                Offset = new Vector2(0.445f, -0.236f)
            },
            new CRoamNearOwner(),
            new CStapleAppliances
            {
                Appliances = new FixedListInt64
                {
                    GDOUtils.GetCustomGameDataObject<PetBed>().ID
                }
            }
        };
    }
}