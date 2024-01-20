using KitchenMods;
using Pets.Interfaces;

namespace Pets.Components.Properties
{
    public struct CRoamNearOwner : IPetProperty, IModComponent
    {
        public float Distance;

        public CRoamNearOwner()
        {
            Distance = 3;
        }
    }
}