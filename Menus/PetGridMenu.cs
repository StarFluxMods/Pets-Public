using System.Collections.Generic;
using Kitchen;
using Kitchen.Modules;
using Pets.Customs.Types;
using Pets.Views;
using UnityEngine;

namespace Pets.Menus
{
    public class PetGridMenu : GridMenu<Pet>
    {
        public PetGridMenu(List<Pet> items, Transform container, int player, bool has_back) : base(items, container, player, has_back)
        {
        }

        protected override int ColumnLength => 3;

        protected override void SetupElement(Pet item, GridMenuElement element)
        {
            element.Set(PrefabSnapshot.GetSnapshot(item.IconPrefab));
        }

        protected override void OnSelect(Pet item)
        {
            if (Player != 0 && item != null)
            {
                PetRequestView.PlayerID = Player;
                PetRequestView.PetID = item.ID;
            }
        }
    }
}