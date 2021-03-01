using Unity.Mathematics;

namespace LacoLico.VoxelTerrain.World.Chunks
{
    public class ChunkStore : PerChunkStore<ChunkProperties>
    {
        public override void GenerateDataForChunkUnchecked(int3 chunkCoordinate)
        {
            ChunkProperties chunkProperties = new ChunkProperties();
            chunkProperties.Initialize(chunkCoordinate, VoxelWorld.WorldSettings.ChunkSize);
            AddChunk(chunkCoordinate, chunkProperties);
        }
        public override void GenerateDataForChunkUnchecked(int3 chunkCoordinate, ChunkProperties existingData)
        {
            existingData.MeshCollider.enabled = false;
            existingData.MeshRenderer.enabled = false;
            existingData.Initialize(chunkCoordinate, VoxelWorld.WorldSettings.ChunkSize);
            AddChunk(chunkCoordinate, existingData);
        }
    }
}