using System.Collections.Generic;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using Pets.Enums;
using Pets.Interfaces;
using Pets.Patches;
using Pets.Views;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Pets.Customs.Types
{
    public class Pet : GameDataObject, IHasPrefab
    {
        protected override void InitialiseDefaults()
        {
            Properties = new List<IPetProperty>();
        }

        GameObject IHasPrefab.Prefab => Prefab;
        public GameObject Prefab;
        public GameObject IconPrefab;
        public ViewType ViewType;
        public PetState DefaultState = PetState.Follow;
        public List<IPetProperty> Properties;

        public override void SetupForGame()
        {
            base.SetupForGame();
            LocalViewRouter_Patch.registeredPetViews.Add(ViewType, Prefab);

            PetView view = Prefab.AddComponent<PetView>();
            view.agent = Prefab.GetComponentInChildren<NavMeshAgent>();
            view.animator = Prefab.GetComponentInChildren<Animator>();
            view.vfx = Prefab.GetComponentInChildren<VisualEffect>();
            view.Colliders = new List<Collider>(Prefab.GetComponentsInChildren<Collider>());
            TextMeshPro tmp = Prefab.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                tmp.transform.parent.gameObject.AddComponent<FixedWorldRotation>();
                view.label = tmp;
            }

            foreach (Transform transform in Prefab.transform)
            {
                if (transform.name != "Error") continue;
                view.warningIcon = transform.gameObject;
                break;
            }
        }
    }

    public abstract class CustomPet : CustomGameDataObject<Pet>
    {
        public virtual GameObject Prefab { get; protected set; }
        public virtual GameObject IconPrefab { get; protected set; }
        public virtual PetState DefaultState { get; protected set; } = PetState.Follow;
        public virtual List<IPetProperty> Properties { get; protected set; } = new List<IPetProperty>();

        public override void Convert(GameData gameData, out GameDataObject gameDataObject)
        {
            Pet result = ScriptableObject.CreateInstance<Pet>();
            
            result.ID = ID;
            result.Prefab = Prefab;
            result.IconPrefab = IconPrefab;
            result.ViewType = (ViewType)VariousUtils.GetID(UniqueNameID);
            result.Properties = Properties;
            result.DefaultState = DefaultState;

            gameDataObject = result;
        }
    }
}