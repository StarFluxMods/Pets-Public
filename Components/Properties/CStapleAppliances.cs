using KitchenMods;
using Pets.Interfaces;
using Unity.Collections;

namespace Pets.Components.Properties
{
    public struct CStapleAppliances : IPetProperty, IModComponent
    {
        public FixedListInt64 Appliances;
    }
}