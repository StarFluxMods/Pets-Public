using System.Collections.Generic;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using UnityEngine;

namespace Pets.Customs
{
    public class PetLetter : CustomAppliance
    {
        public override string UniqueNameID => "PetLetter";

        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("PetLetter").AssignMaterialsByNames();

        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>
        {
            new CFixedRotation(),
            new CImmovable()
        };

        public override void OnRegister(Appliance gameDataObject)
        {
            base.OnRegister(gameDataObject);
            LetterView view = gameDataObject.Prefab.AddComponent<LetterView>();
            Appliance Letter = GDOUtils.GetExistingGDO(ApplianceReferences.BlueprintLetter) as Appliance;
            view.Animator = gameDataObject.Prefab.GetComponent<Animator>();
            view.Letter = GameObjectUtils.GetChild(gameDataObject.Prefab, "Letter");
            view.MinDelay = 0;
            view.MaxDelay = 2;
        }
    }
}