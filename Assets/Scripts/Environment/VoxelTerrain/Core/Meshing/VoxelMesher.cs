using LacoLico.VoxelTerrain.Utilities;
using LacoLico.VoxelTerrain.VoxelData;
using LacoLico.VoxelTerrain.World;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Meshing
{
    public abstract class VoxelMesher : MonoBehaviour
    {
        public VoxelWorld VoxelWorld { get; set; }
        public abstract JobHandleWithData<IMesherJob> CreateMesh(VoxelDataStore voxelDataStore, VoxelColorStore voxelColorStore, int3 chunkCoordinate);
    }
}