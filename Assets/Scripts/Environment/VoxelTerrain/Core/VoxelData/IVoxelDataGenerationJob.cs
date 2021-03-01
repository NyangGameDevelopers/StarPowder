using Unity.Jobs;
using Unity.Mathematics;

namespace LacoLico.VoxelTerrain.VoxelData
{
    public interface IVoxelDataGenerationJob : IJob
    {
        int3 WorldPositionOffset { get; set; }
        VoxelDataVolume<byte> OutputVoxelData { get; set; }
    }
}