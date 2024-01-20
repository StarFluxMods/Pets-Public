using Pets.Enums;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Components
{
    public struct CPet : IComponentData
    {
        public PetState State;
        public Entity Owner;
        public int PetLinkId;
        public int PetType;
        public FixedString64 PetName;
    }
}