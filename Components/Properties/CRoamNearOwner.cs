using Pets.Interfaces;

namespace Pets.Components.Properties
{
    public struct CRoamNearOwner : IPetProperty
    {
        public float Distance;

        public CRoamNearOwner()
        {
            Distance = 3;
        }
    }
}