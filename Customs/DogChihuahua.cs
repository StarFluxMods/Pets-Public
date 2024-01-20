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
    public class DogChihuahua : CustomPet
    {
        public override string UniqueNameID => "DogChihuahua";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("DogChihuahua").AssignMaterialsByNames();
        public override GameObject IconPrefab => Mod.Bundle.LoadAsset<GameObject>("DogChihuahuaIcon").AssignMaterialsByNames();
        public override PetState DefaultState => PetState.Follow;

        public override List<IPetProperty> Properties { get; protected set; } = new List<IPetProperty>()
        {
            new CActivities
            {
                Activities = new FixedListInt64
                {
                    (int)PetState.Follow,
                    (int)PetState.Eat
                }
            },
            new CRoamNearOwner()
        };
    }
}