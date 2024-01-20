using KitchenMods;
using Pets.Interfaces;
using Unity.Collections;

namespace Pets.Components.Properties
{
    public struct CPreferredFoods : IPetProperty, IModComponent
    {
        public FixedListInt32 PreferredFoods;
    }
}