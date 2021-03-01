using LacoLico.VoxelTerrain.Utilities;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.World.Chunks
{
    public class ChunkProvider : MonoBehaviour
    {
        public VoxelWorld VoxelWorld { get; set; }
        protected ChunkProperties CreateUnloadedChunkToCoordinate(int3 chunkCoordinate)
        {
            int3 worldPosition = chunkCoordinate * VoxelWorld.WorldSettings.ChunkSize;
            GameObject chunkGameObject = Instantiate(VoxelWorld.WorldSettings.ChunkPrefab, worldPosition.ToVectorInt(), Quaternion.identity);

            ChunkProperties chunkProperties = new ChunkProperties
            {
                ChunkGameObject = chunkGameObject,
                MeshCollider = chunkGameObject.GetComponent<MeshCollider>(),
                MeshFilter = chunkGameObject.GetComponent<MeshFilter>(),
                MeshRenderer = chunkGameObject.GetComponent<MeshRenderer>()
            };

            chunkProperties.Initialize(chunkCoordinate, VoxelWorld.WorldSettings.ChunkSize);

            VoxelWorld.ChunkStore.AddChunk(chunkCoordinate, chunkProperties);

            return chunkProperties;
        }
        public ChunkProperties CreateLoadedChunkToCoordinateImmediate(int3 chunkCoordinate)
        {
            ChunkProperties chunkProperties = CreateUnloadedChunkToCoordinate(chunkCoordinate);
            VoxelWorld.ChunkUpdater.GenerateVoxelDataAndMeshImmediate(chunkProperties);
            return chunkProperties;
        }
    }
}