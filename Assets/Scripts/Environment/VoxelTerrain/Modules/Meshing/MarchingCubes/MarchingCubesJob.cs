using LacoLico.VoxelTerrain.Meshing.Data;
using LacoLico.VoxelTerrain.VoxelData;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Meshing.MarchingCubes
{
    [BurstCompile]
    public struct MarchingCubesJob : IMesherJob
    {
        [ReadOnly] private VoxelDataVolume<byte> _voxelData;
        [ReadOnly] private VoxelDataVolume<Color32> _voxelColors;
        public float Isolevel { get; set; }
        public NativeCounter VertexCountCounter { get; set; }
        [NativeDisableParallelForRestriction, WriteOnly] private NativeArray<MeshingVertexData> _vertices;
        [NativeDisableParallelForRestriction, WriteOnly] private NativeArray<ushort> _triangles;
        public VoxelDataVolume<byte> VoxelData { get => _voxelData; set => _voxelData = value; }
        public VoxelDataVolume<Color32> VoxelColors { get => _voxelColors; set => _voxelColors = value; }
        public NativeArray<MeshingVertexData> OutputVertices { get => _vertices; set => _vertices = value; }
        public NativeArray<ushort> OutputTriangles { get => _triangles; set => _triangles = value; }
        public void Execute()
        {
            byte isolevelByte = (byte)math.clamp(Isolevel * 255, 0, 255);
            for (int x = 0; x < VoxelData.Width - 1; x++)
            {
                for (int y = 0; y < VoxelData.Height - 1; y++)
                {
                    for (int z = 0; z < VoxelData.Depth - 1; z++)
                    {
                        int3 voxelLocalPosition = new int3(x, y, z);

                        VoxelCorners<byte> densities = _voxelData.GetVoxelDataUnitCube(voxelLocalPosition);

                        byte cubeIndex = MarchingCubesFunctions.CalculateCubeIndex(densities, isolevelByte);
                        if (cubeIndex == 0 || cubeIndex == 255)
                        {
                            continue;
                        }

                        int edgeIndex = MarchingCubesLookupTables.EdgeTable[cubeIndex];

                        VertexList vertexList = MarchingCubesFunctions.GenerateVertexList(densities, voxelLocalPosition, edgeIndex, isolevelByte);

                        // Index at the beginning of the row
                        int rowIndex = 15 * cubeIndex;

                        for (int i = 0; MarchingCubesLookupTables.TriangleTable[rowIndex + i] != -1 && i < 15; i += 3)
                        {
                            float3 vertex1 = vertexList[MarchingCubesLookupTables.TriangleTable[rowIndex + i + 0]];
                            float3 vertex2 = vertexList[MarchingCubesLookupTables.TriangleTable[rowIndex + i + 1]];
                            float3 vertex3 = vertexList[MarchingCubesLookupTables.TriangleTable[rowIndex + i + 2]];

                            if (!vertex1.Equals(vertex2) && !vertex1.Equals(vertex3) && !vertex2.Equals(vertex3))
                            {
                                float3 normal = math.normalize(math.cross(vertex2 - vertex1, vertex3 - vertex1));

                                int triangleIndex = VertexCountCounter.Increment() * 3;

                                float3 triangleMiddlePoint = (vertex1 + vertex2 + vertex3) / 3f;

                                // Take the position of the closest corner of the current voxel
                                int3 colorSamplePoint = (int3)math.round(triangleMiddlePoint);
                                Color32 color = VoxelColors.GetVoxelData(colorSamplePoint);

                                _vertices[triangleIndex + 0] = new MeshingVertexData(vertex1, normal, color);
                                _triangles[triangleIndex + 0] = (ushort)(triangleIndex + 0);

                                _vertices[triangleIndex + 1] = new MeshingVertexData(vertex2, normal, color);
                                _triangles[triangleIndex + 1] = (ushort)(triangleIndex + 1);

                                _vertices[triangleIndex + 2] = new MeshingVertexData(vertex3, normal, color);
                                _triangles[triangleIndex + 2] = (ushort)(triangleIndex + 2);
                            }
                        }
                    }
                }
            }
        }
    }
}