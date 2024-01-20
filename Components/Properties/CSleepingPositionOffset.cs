using KitchenMods;
using Pets.Interfaces;
using UnityEngine;

namespace Pets.Components.Properties
{
    public struct CSleepingPositionOffset : IPetProperty, IModComponent
    {
        public Vector2 Offset;
    }
}