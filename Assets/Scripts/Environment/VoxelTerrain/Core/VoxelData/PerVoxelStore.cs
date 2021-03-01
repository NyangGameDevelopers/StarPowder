using LacoLico.VoxelTerrain.Utilities;
using LacoLico.VoxelTerrain.Utilities.Intersection;
using LacoLico.VoxelTerrain.VoxelData;
using LacoLico.VoxelTerrain.World.Chunks;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.World
{
    public abstract class PerVoxelStore<T> : PerChunkStore<VoxelDataVolume<T>> where T : struct
    {
        protected virtual void OnApplicationQuit()
        {
            foreach (VoxelDataVolume<T> dataArray in _chunks.Values)
            {
                if (dataArray.IsCreated)
                {
                    dataArray.Dispose();
                }
            }
        }
        public void SetData(int3 dataWorldPosition, T dataValue)
        {
            int3[] affectedChunkCoordinates = CoordinateUtilities.GetChunkCoordinatesContainingPoint(dataWorldPosition, VoxelWorld.WorldSettings.ChunkSize);

            for (int i = 0; i < affectedChunkCoordinates.Length; i++)
            {
                int3 chunkCoordinate = affectedChunkCoordinates[i];
                if (TryGetDataChunk(chunkCoordinate, out VoxelDataVolume<T> chunkData))
                {
                    int3 localPos = (dataWorldPosition - chunkCoordinate * VoxelWorld.WorldSettings.ChunkSize).Mod(VoxelWorld.WorldSettings.ChunkSize + 1);

                    chunkData.SetVoxelData(dataValue, localPos);

                    if (VoxelWorld.ChunkStore.TryGetDataChunk(chunkCoordinate, out ChunkProperties chunkProperties))
                    {
                        chunkProperties.HasChanges = true;
                    }
                }
            }
        }
        public virtual void SetDataChunk(int3 chunkCoordinate, VoxelDataVolume<T> newData)
        {
            bool dataExistsAtCoordinate = DoesChunkExistAtCoordinate(chunkCoordinate);
            SetDataChunkUnchecked(chunkCoordinate, newData, dataExistsAtCoordinate);
        }
        public virtual void SetDataChunkUnchecked(int3 chunkCoordinate, VoxelDataVolume<T> newData, bool dataExistsAtCoordinate)
        {
            if (dataExistsAtCoordinate)
            {
                _chunks[chunkCoordinate].CopyFrom(newData);
                newData.Dispose();
            }
            else
            {
                _chunks.Add(chunkCoordinate, newData);
            }

            if (VoxelWorld.ChunkStore.TryGetDataChunk(chunkCoordinate, out ChunkProperties chunkProperties))
            {
                chunkProperties.HasChanges = true;
            }
        }
        public override void GenerateDataForChunkUnchecked(int3 chunkCoordinate)
        {
            VoxelDataVolume<T> data = new VoxelDataVolume<T>(VoxelWorld.WorldSettings.ChunkSize + 1, Allocator.Persistent);
            GenerateDataForChunkUnchecked(chunkCoordinate, data);
        }
        public void ForEachVoxelDataInQueryInChunk(BoundsInt worldSpaceQuery, int3 chunkCoordinate, VoxelDataVolume<T> dataChunk, Action<int3, int3, int, T> function)
        {
            int3 chunkBoundsSize = VoxelWorld.WorldSettings.ChunkSize;
            int3 chunkWorldSpaceOrigin = chunkCoordinate * VoxelWorld.WorldSettings.ChunkSize;

            BoundsInt chunkWorldSpaceBounds = new BoundsInt(chunkWorldSpaceOrigin.ToVectorInt(), chunkBoundsSize.ToVectorInt());

            BoundsInt intersectionVolume = IntersectionUtilities.GetIntersectionVolume(worldSpaceQuery, chunkWorldSpaceBounds);
            int3 intersectionVolumeMin = intersectionVolume.min.ToInt3();
            int3 intersectionVolumeMax = intersectionVolume.max.ToInt3();

            for (int voxelDataWorldPositionX = intersectionVolumeMin.x; voxelDataWorldPositionX <= intersectionVolumeMax.x; voxelDataWorldPositionX++)
            {
                for (int voxelDataWorldPositionY = intersectionVolumeMin.y; voxelDataWorldPositionY <= intersectionVolumeMax.y; voxelDataWorldPositionY++)
                {
                    for (int voxelDataWorldPositionZ = intersectionVolumeMin.z; voxelDataWorldPositionZ <= intersectionVolumeMax.z; voxelDataWorldPositionZ++)
                    {
                        int3 voxelDataWorldPosition = new int3(voxelDataWorldPositionX, voxelDataWorldPositionY, voxelDataWorldPositionZ);

                        int3 voxelDataLocalPosition = voxelDataWorldPosition - chunkWorldSpaceOrigin;
                        int voxelDataIndex = IndexUtilities.XyzToIndex(voxelDataLocalPosition, chunkBoundsSize.x + 1, chunkBoundsSize.y + 1);
                        if (dataChunk.TryGetVoxelData(voxelDataIndex, out T voxelData))
                        {
                            function(voxelDataWorldPosition, voxelDataLocalPosition, voxelDataIndex, voxelData);
                        }
                    }
                }
            }
        }
        public bool TryGetVoxelData(int3 worldPosition, out T voxelData)
        {
            int3 chunkCoordinate = VectorUtilities.WorldPositionToCoordinate(worldPosition, VoxelWorld.WorldSettings.ChunkSize);
            if (TryGetDataChunk(chunkCoordinate, out VoxelDataVolume<T> chunk))
            {
                int3 voxelDataLocalPosition = worldPosition.Mod(VoxelWorld.WorldSettings.ChunkSize);
                return chunk.TryGetVoxelData(voxelDataLocalPosition, out voxelData);
            }
            else
            {
                voxelData = default;
                return false;
            }
        }
        public VoxelDataVolume<T> GetVoxelDataCustom(BoundsInt worldSpaceQuery, Allocator allocator)
        {
            VoxelDataVolume<T> voxelDataArray = new VoxelDataVolume<T>(worldSpaceQuery.CalculateVolume(), allocator);

            ForEachVoxelDataArrayInQuery(worldSpaceQuery, (chunkCoordinate, voxelDataChunk) =>
            {
                ForEachVoxelDataInQueryInChunk(worldSpaceQuery, chunkCoordinate, voxelDataChunk, (voxelDataWorldPosition, voxelDataLocalPosition, voxelDataIndex, voxelData) =>
                {
                    voxelDataArray.SetVoxelData(voxelData, voxelDataWorldPosition - worldSpaceQuery.min.ToInt3());
                });
            });

            return voxelDataArray;
        }
        public void SetVoxelDataCustom(VoxelDataVolume<T> voxelDataArray, int3 originPosition)
        {
            BoundsInt worldSpaceQuery = new BoundsInt(originPosition.ToVectorInt(), (voxelDataArray.Size - new int3(1, 1, 1)).ToVectorInt());

            ForEachVoxelDataArrayInQuery(worldSpaceQuery, (chunkCoordinate, voxelDataChunk) =>
            {
                ForEachVoxelDataInQueryInChunk(worldSpaceQuery, chunkCoordinate, voxelDataChunk, (voxelDataWorldPosition, voxelDataLocalPosition, voxelDataIndex, voxelData) =>
                {
                    voxelDataChunk.SetVoxelData(voxelData, voxelDataWorldPosition - chunkCoordinate * VoxelWorld.WorldSettings.ChunkSize);
                });

                if (VoxelWorld.ChunkStore.TryGetDataChunk(chunkCoordinate, out ChunkProperties chunkProperties))
                {
                    chunkProperties.HasChanges = true;
                }
            });
        }
        public void SetVoxelDataCustom(BoundsInt worldSpaceQuery, Func<int3, T, T> setVoxelDataFunction)
        {
            ForEachVoxelDataArrayInQuery(worldSpaceQuery, (chunkCoordinate, voxelDataChunk) =>
            {
                bool anyChanged = false;
                ForEachVoxelDataInQueryInChunk(worldSpaceQuery, chunkCoordinate, voxelDataChunk, (voxelDataWorldPosition, voxelDataLocalPosition, voxelDataIndex, voxelData) =>
                {
                    T newVoxelData = setVoxelDataFunction(voxelDataWorldPosition, voxelData);
                    if (!newVoxelData.Equals(voxelData))
                    {
                        voxelDataChunk.SetVoxelData(newVoxelData, voxelDataIndex);
                        anyChanged = true;
                    }
                });

                if (anyChanged)
                {
                    if (VoxelWorld.ChunkStore.TryGetDataChunk(chunkCoordinate, out ChunkProperties chunkProperties))
                    {
                        chunkProperties.HasChanges = true;
                    }
                }
            });
        }
    }
}