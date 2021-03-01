using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Settings
{
    [System.Serializable]
    public class WorldSettings
    {
        [SerializeField] private int3 chunkSize = new int3(16, 16, 16);
        [SerializeField] private GameObject chunkPrefab;
        public int3 ChunkSize => chunkSize;
        public GameObject ChunkPrefab => chunkPrefab;
    }
}