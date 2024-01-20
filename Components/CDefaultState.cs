using KitchenMods;
using Pets.Enums;

namespace Pets.Components
{
    public struct CDefaultState : IModComponent
    {
        public CDefaultState(PetState state)
        {
            State = state;
        }
        
        public readonly PetState State;
    }
}