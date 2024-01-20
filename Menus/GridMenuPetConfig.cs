using System.Collections.Generic;
using Kitchen.Modules;
using Pets.Customs.Types;
using UnityEngine;

namespace Pets.Menus
{
    public class GridMenuPetConfig : GridMenuConfig
    {
        public override GridMenu Instantiate(Transform container, int player, bool has_back)
        {
            return new PetGridMenu(Pets, container, player, has_back);
        }

        public List<Pet> Pets = new List<Pet>();
    }
}