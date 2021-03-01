using LacoLico.VoxelTerrain.Utilities;
using System.Runtime.CompilerServices;
using LacoLico.VoxelTerrain.Meshing.Data;
using Unity.Mathematics;
using LacoLico.VoxelTerrain.VoxelData;

namespace LacoLico.VoxelTerrain.Meshing.MarchingCubes
{
    public static class MarchingCubesFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 VertexInterpolate(float3 p1, float3 p2, float v1, float v2, float isolevel)
        {
            return p1 + (isolevel - v1) * (p2 - p1) / (v2 - v1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte CalculateCubeIndex(VoxelCorners<byte> voxelDensities, byte isolevel)
        {
            byte cubeIndex = (byte)math.select(0, 1, voxelDensities.Corner1 < isolevel);
            cubeIndex |= (byte)math.select(0, 2, voxelDensities.Corner2 < isolevel);
            cubeIndex |= (byte)math.select(0, 4, voxelDensities.Corner3 < isolevel);
            cubeIndex |= (byte)math.select(0, 8, voxelDensities.Corner4 < isolevel);
            cubeIndex |= (byte)math.select(0, 16, voxelDensities.Corner5 < isolevel);
            cubeIndex |= (byte)math.select(0, 32, voxelDensities.Corner6 < isolevel);
            cubeIndex |= (byte)math.select(0, 64, voxelDensities.Corner7 < isolevel);
            cubeIndex |= (byte)math.select(0, 128, voxelDensities.Corner8 < isolevel);

            return cubeIndex;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VertexList GenerateVertexList(VoxelCorners<byte> voxelDensities, int3 voxelLocalPosition,
            int edgeIndex, byte isolevel)
        {
            VertexList vertexList = new VertexList();

            for (int i = 0; i < 12; i++)
            {
                if ((edgeIndex & (1 << i)) == 0) { continue; }

                int edgeStartIndex = MarchingCubesLookupTables.EdgeIndexTable[2 * i + 0];
                int edgeEndIndex = MarchingCubesLookupTables.EdgeIndexTable[2 * i + 1];

                int3 corner1 = voxelLocalPosition + LookupTables.CubeCorners[edgeStartIndex];
                int3 corner2 = voxelLocalPosition + LookupTables.CubeCorners[edgeEndIndex];

                float density1 = voxelDensities[edgeStartIndex] / 255f;
                float density2 = voxelDensities[edgeEndIndex] / 255f;

                vertexList[i] = VertexInterpolate(corner1, corner2, density1, density2, isolevel / 255f);
            }

            return vertexList;
        }
        public static VoxelCorners<T> GetVoxelDataUnitCube<T>(this VoxelDataVolume<T> voxelDataArray, int3 localPosition) where T : struct
        {
            VoxelCorners<T> voxelDataCorners = new VoxelCorners<T>();
            for (int i = 0; i < 8; i++)
            {
                int3 voxelCorner = localPosition + LookupTables.CubeCorners[i];
                if (voxelDataArray.TryGetVoxelData(voxelCorner, out T voxelData))
                {
                    voxelDataCorners[i] = voxelData;
                }
            }

            return voxelDataCorners;
        }
    }
}