using LacoLico.VoxelTerrain.Utilities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.VoxelData
{
    public abstract class VoxelDataGenerator : MonoBehaviour
    {
        public JobHandleWithData<IVoxelDataGenerationJob> GenerateVoxelData(BoundsInt bounds)
        {
            return GenerateVoxelData(bounds, Allocator.Persistent);
        }
        public JobHandleWithData<IVoxelDataGenerationJob> GenerateVoxelData(BoundsInt bounds, Allocator allocator)
        {
            VoxelDataVolume<byte> voxelDataArray = new VoxelDataVolume<byte>(bounds.size, allocator);
            int3 worldSpaceOrigin = bounds.min.ToInt3();
            return GenerateVoxelData(worldSpaceOrigin, voxelDataArray);
        }
        public abstract JobHandleWithData<IVoxelDataGenerationJob> GenerateVoxelData(int3 worldSpaceOrigin, VoxelDataVolume<byte> outputVoxelDataArray);
    }
}