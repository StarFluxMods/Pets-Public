using System.Collections.Generic;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using Pets.Components;
using UnityEngine;

namespace Pets.Customs
{
    public class PetBed : CustomAppliance
    {
        public override string UniqueNameID => "PetBed";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("PetBed").AssignMaterialsByNames();
        public override string Name => "Pet Bed";
        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>
        {
            new CPetBed()
        };
    }
}