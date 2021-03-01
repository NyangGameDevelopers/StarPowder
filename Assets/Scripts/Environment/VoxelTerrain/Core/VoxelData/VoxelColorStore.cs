using LacoLico.VoxelTerrain.World;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.VoxelData
{
    public class VoxelColorStore : PerVoxelStore<Color32>
    {
        [SerializeField] private Color32 defaultTerrainColor = new Color32(11, 91, 33, 255);
        public override unsafe void GenerateDataForChunkUnchecked(int3 chunkCoordinate, VoxelDataVolume<Color32> outputColors)
        {
            Color32* defaultColorArray = stackalloc Color32[1]
            {
                defaultTerrainColor
            };

            unsafe
            {
                UnsafeUtility.MemCpyReplicate(outputColors.GetUnsafePtr(), defaultColorArray, sizeof(Color32), outputColors.Length);
            }

            SetDataChunkUnchecked(chunkCoordinate, outputColors, false);
        }
    }
}
