using Pets.Interfaces;

namespace Pets.Components.Properties
{
    public struct CLonelyDistance : IPetProperty
    {
        public float Distance;

        public CLonelyDistance()
        {
            Distance = 4.5f;
        }
    }
}