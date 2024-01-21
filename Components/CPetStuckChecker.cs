using KitchenMods;
using UnityEngine;

namespace Pets.Components
{
    public struct CPetStuckChecker : IModComponent
    {
        public long LastCheck;
        public Vector3 LastPosition;
    }
}