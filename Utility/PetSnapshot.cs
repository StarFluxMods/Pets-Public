using Kitchen;
using UnityEngine;

namespace Pets.Utility
{
    public static class PetSnapshot
    {
        public static Texture2D GetApplianceSnapshot(GameObject prefab)
        {
            int instanceID = prefab.GetInstanceID();
            Quaternion rotation = Quaternion.LookRotation(new Vector3(-1f, 1f, -1f), new Vector3(0f, 1f, 1f));
            SnapshotTexture snapshotTexture = Snapshot.RenderPrefabToTexture(512, 512, prefab, rotation, 0.5f, 0.5f, -10f, 10f, 1f, -0.25f * new Vector3(0f, 0, 1f));
            return snapshotTexture.Snapshot;
        }
    }
}