using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Utilities
{
    public static class BoundsUtilities
    {
        public static BoundsInt GetChunkBounds(int3 chunkCoordinate, int3 chunkSize)
        {
            int3 min = chunkCoordinate * chunkSize;
            int3 size = new int3(1, 1, 1) * (chunkSize + new int3(1, 1, 1));

            return new BoundsInt(min.ToVectorInt(), size.ToVectorInt());
        }
        public static int CalculateVolume(this BoundsInt bounds)
        {
            Vector3Int size = bounds.size;
            return size.x * size.y * size.z;
        }
    }
}