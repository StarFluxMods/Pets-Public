using Pets.Interfaces;
using Unity.Collections;

namespace Pets.Components.Properties
{
    public struct CPreferredFoods : IPetProperty
    {
        public FixedListInt32 PreferredFoods;
    }
}