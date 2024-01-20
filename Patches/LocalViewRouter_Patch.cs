using System.Collections.Generic;
using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace Pets.Patches
{
    [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
    public class LocalViewRouter_Patch
    {
        public static Dictionary<ViewType, GameObject> registeredPetViews = new();
		
        static bool Prefix(LocalViewRouter __instance, ViewType view_type, ref GameObject __result)
        {
            if (!registeredPetViews.TryGetValue(view_type, out GameObject petPrefab)) return true;
            __result = petPrefab;
            return false;
        }
    }
}