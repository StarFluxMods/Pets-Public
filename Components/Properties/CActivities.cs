using Pets.Interfaces;
using Unity.Collections;

namespace Pets.Components.Properties
{
    public struct CActivities : IPetProperty
    {
        public FixedListInt64 Activities;
    }
}