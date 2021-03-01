using System.Collections.Generic;
using System.Linq;
using LacoLico.VoxelTerrain.Utilities;
using LacoLico.VoxelTerrain.World;
using LacoLico.VoxelTerrain.World.Chunks;
using Unity.Mathematics;

namespace LacoLico.VoxelTerrain.VoxelData
{
    public class VoxelDataStore : PerVoxelStore<byte>
    {
        private Dictionary<int3, JobHandleWithData<IVoxelDataGenerationJob>> _generationJobHandles;
        protected override void Awake()
        {
            base.Awake();
            _generationJobHandles = new Dictionary<int3, JobHandleWithData<IVoxelDataGenerationJob>>();
        }
        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            if (_generationJobHandles != null)
            {
                foreach (var jobHandle in _generationJobHandles.Values)
                {
                    jobHandle.JobHandle.Complete();
                    jobHandle.JobData.OutputVoxelData.Dispose();
                }

                _generationJobHandles.Clear();
            }
        }
        public override bool DoesChunkExistAtCoordinate(int3 chunkCoordinate)
        {
            return base.DoesChunkExistAtCoordinate(chunkCoordinate) || _generationJobHandles.ContainsKey(chunkCoordinate);
        }
        public override IEnumerable<int3> GetChunkCoordinatesOutsideOfRange(int3 coordinate, int range)
        {
            foreach(var baseCoordinate in base.GetChunkCoordinatesOutsideOfRange(coordinate, range))
            {
                yield return baseCoordinate;
            }

            int3[] generationJobHandleArray = _generationJobHandles.Keys.ToArray();
            for (int i = 0; i < generationJobHandleArray.Length; i++)
            {
                int3 generationCoordinate = generationJobHandleArray[i];
                if (DistanceUtilities.ChebyshevDistanceGreaterThan(coordinate, generationCoordinate, range))
                {
                    yield return generationCoordinate;
                }
            }
        }
        public override bool TryGetDataChunk(int3 chunkCoordinate, out VoxelDataVolume<byte> chunk)
        {
            ApplyChunkChanges(chunkCoordinate);
            return TryGetDataChunkWithoutApplying(chunkCoordinate, out chunk);
        }
        private bool TryGetDataChunkWithoutApplying(int3 chunkCoordinate, out VoxelDataVolume<byte> chunk)
        {
            return _chunks.TryGetValue(chunkCoordinate, out chunk);
        }
        private void ApplyChunkChanges(int3 chunkCoordinate)
        {
            if (_generationJobHandles.TryGetValue(chunkCoordinate, out JobHandleWithData<IVoxelDataGenerationJob> jobHandle))
            {
                jobHandle.JobHandle.Complete();
                SetDataChunk(chunkCoordinate, jobHandle.JobData.OutputVoxelData);
                _generationJobHandles.Remove(chunkCoordinate);
            }
        }
        public override void SetDataChunk(int3 chunkCoordinate, VoxelDataVolume<byte> newData)
        {
            if (TryGetDataChunkWithoutApplying(chunkCoordinate, out VoxelDataVolume<byte> oldData))
            {
                oldData.CopyFrom(newData);
                newData.Dispose();
            }
            else
            {
                AddChunkUnchecked(chunkCoordinate, newData);
            }

            if (VoxelWorld.ChunkStore.TryGetDataChunk(chunkCoordinate, out ChunkProperties chunkProperties))
            {
                chunkProperties.HasChanges = true;
            }
        }
        public override void GenerateDataForChunkUnchecked(int3 chunkCoordinate, VoxelDataVolume<byte> existingData)
        {
            int3 chunkWorldOrigin = chunkCoordinate * VoxelWorld.WorldSettings.ChunkSize;
            JobHandleWithData<IVoxelDataGenerationJob> jobHandleWithData = VoxelWorld.VoxelDataGenerator.GenerateVoxelData(chunkWorldOrigin, existingData);
            _generationJobHandles.Add(chunkCoordinate, jobHandleWithData);
        }
    }
}
