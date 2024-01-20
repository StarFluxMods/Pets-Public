using KitchenMods;
using Pets.Interfaces;
using Unity.Collections;

namespace Pets.Components.Properties
{
    public struct CActivities : IPetProperty, IModComponent
    {
        public FixedListInt64 Activities;
    }
}