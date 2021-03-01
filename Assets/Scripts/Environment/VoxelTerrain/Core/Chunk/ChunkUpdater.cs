using LacoLico.VoxelTerrain.Meshing;
using LacoLico.VoxelTerrain.Meshing.Data;
using LacoLico.VoxelTerrain.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace LacoLico.VoxelTerrain.World.Chunks
{
    public class ChunkUpdater : MonoBehaviour
    {
        public VoxelWorld VoxelWorld { get; set; }

        private void Update()
        {
            foreach (ChunkProperties chunkProperties in VoxelWorld.ChunkStore.Chunks)
            {
                if (chunkProperties.HasChanges)
                {
                    GenerateMeshImmediate(chunkProperties);
                }
            }
        }
        public void GenerateVoxelDataAndMeshImmediate(ChunkProperties chunkProperties)
        {
            VoxelWorld.VoxelDataStore.GenerateDataForChunk(chunkProperties.ChunkCoordinate);
            VoxelWorld.VoxelColorStore.GenerateDataForChunk(chunkProperties.ChunkCoordinate);
            GenerateMeshImmediate(chunkProperties);
        }
        public void GenerateMeshImmediate(ChunkProperties chunkProperties)
        {
            JobHandleWithData<IMesherJob> jobHandleWithData = VoxelWorld.VoxelMesher.CreateMesh(VoxelWorld.VoxelDataStore, VoxelWorld.VoxelColorStore, chunkProperties.ChunkCoordinate);
            if (jobHandleWithData == null) { return; }

            IMesherJob job = jobHandleWithData.JobData;

            Mesh mesh = new Mesh();
            SubMeshDescriptor subMesh = new SubMeshDescriptor(0, 0);

            jobHandleWithData.JobHandle.Complete();

            int vertexCount = job.VertexCountCounter.Count * 3;
            job.VertexCountCounter.Dispose();

            mesh.SetVertexBufferParams(vertexCount, MeshingVertexData.VertexBufferMemoryLayout);
            mesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt16);

            mesh.SetVertexBufferData(job.OutputVertices, 0, 0, vertexCount, 0, MeshUpdateFlags.DontValidateIndices);
            mesh.SetIndexBufferData(job.OutputTriangles, 0, 0, vertexCount, MeshUpdateFlags.DontValidateIndices);

            job.OutputVertices.Dispose();
            job.OutputTriangles.Dispose();

            mesh.subMeshCount = 1;
            subMesh.indexCount = vertexCount;
            mesh.SetSubMesh(0, subMesh);

            mesh.RecalculateBounds();

            chunkProperties.MeshFilter.sharedMesh = mesh;
            chunkProperties.MeshCollider.sharedMesh = mesh;

            chunkProperties.MeshCollider.enabled = true;
            chunkProperties.MeshRenderer.enabled = true;

            chunkProperties.HasChanges = false;

            chunkProperties.IsMeshGenerated = true;
        }
    }
}
