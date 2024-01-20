using KitchenMods;
using Pets.Interfaces;

namespace Pets.Components.Properties
{
    public struct CLonelyDistance : IPetProperty, IModComponent
    {
        public float Distance;

        public CLonelyDistance()
        {
            Distance = 4.5f;
        }
    }
}