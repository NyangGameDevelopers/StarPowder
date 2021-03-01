using LacoLico.VoxelTerrain.Meshing.Data;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using LacoLico.VoxelTerrain.VoxelData;

namespace LacoLico.VoxelTerrain.Meshing
{
    public interface IMesherJob : IJob
    {
        NativeCounter VertexCountCounter { get; set; }
        VoxelDataVolume<byte> VoxelData { get; set; }
        VoxelDataVolume<Color32> VoxelColors { get; set; }
        NativeArray<MeshingVertexData> OutputVertices { get; set; }
        NativeArray<ushort> OutputTriangles { get; set; }
    }
}