using KitchenMods;
using Unity.Entities;

namespace Pets.Components.Menu
{
    public struct CTriggerPetEditor : IModComponent
    {
        public bool IsTriggered;
        public Entity TriggerEntity;
    }
}