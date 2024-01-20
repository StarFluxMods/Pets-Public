using KitchenMods;
using Unity.Entities;

namespace Pets.Components.Status
{
    public struct CPetInteractingWith : IComponentData, IModComponent
    {
        public Entity InteractingWith;
        public long StartTime;
        public long TimeToFinish;
        public bool IsWaitingForDestination;
    }
}