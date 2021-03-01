using LacoLico.VoxelTerrain.Utilities;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.World.Chunks
{
    // Chunk 속성
    public class ChunkProperties
    {
        public GameObject ChunkGameObject { get; set; }
        public MeshFilter MeshFilter { get; set; }
        public MeshCollider MeshCollider { get; set; }
        public MeshRenderer MeshRenderer { get; set; }

        public int3 ChunkCoordinate { get; set; }

        public bool HasChanges { get; set; }
        public bool IsMeshGenerated { get; set; }

        public void Initialize(int3 chunkCoordinate, int3 chunkSize)
        {
            #if UNITY_EDITOR
            ChunkGameObject.name = GetName(chunkCoordinate);
            #endif
            
            ChunkGameObject.transform.position = (chunkCoordinate * chunkSize).ToVectorInt();
            ChunkCoordinate = chunkCoordinate;

            IsMeshGenerated = false;
            HasChanges = false;
        }

        public static string GetName(int3 chunkCoordinate)
        {
            return $"Chunk_{chunkCoordinate.x}_{chunkCoordinate.y}_{chunkCoordinate.z}";
        }
    }
}