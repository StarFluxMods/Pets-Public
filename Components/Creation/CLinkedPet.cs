using KitchenMods;
using Unity.Entities;

namespace Pets.Components.Creation
{
    public struct CLinkedPet : IModComponent
    {
        public int PetType;
        public Entity PetEntity;
        public int PetLinkId;
    }
}