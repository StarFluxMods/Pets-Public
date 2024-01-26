using System.Collections.Generic;
using Kitchen;
using Kitchen.Modules;
using KitchenData;
using Pets.Customs.Types;
using Pets.Views;
using UnityEngine;

namespace Pets.Menus
{
    public class PetGridMenu : GridMenu<int>
    {
        public PetGridMenu(List<int> items, Transform container, int player, bool has_back) : base(items, container, player, has_back)
        {
        }

        protected override int ColumnLength => 3;

        protected override void SetupElement(int item, GridMenuElement element)
        {
            if (item == 0)
            {
                element.Set(Mod.Bundle.LoadAsset<Texture2D>("None"));
                return;
            }
            element.Set(PrefabSnapshot.GetSnapshot(GameData.Main.Get<Pet>(item).IconPrefab));
        }

        protected override void OnSelect(int item)
        {
            if (Player != 0 && item != null)
            {
                PetRequestView.PlayerID = Player;
                PetRequestView.PetID = item;
            }
        }
    }
}