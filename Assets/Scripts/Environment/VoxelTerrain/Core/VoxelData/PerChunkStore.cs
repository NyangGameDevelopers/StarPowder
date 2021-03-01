using LacoLico.VoxelTerrain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.World
{
    public abstract class PerChunkStore<T> : MonoBehaviour
    {
        public VoxelWorld VoxelWorld { get; set; }
        public IEnumerable<T> Chunks => _chunks.Values;
        protected Dictionary<int3, T> _chunks;
        protected virtual void Awake()
        {
            _chunks = new Dictionary<int3, T>();
        }
        public virtual bool DoesChunkExistAtCoordinate(int3 chunkCoordinate)
        {
            return _chunks.ContainsKey(chunkCoordinate);
        }
        public virtual bool TryGetDataChunk(int3 chunkCoordinate, out T chunkData)
        {
            return _chunks.TryGetValue(chunkCoordinate, out chunkData);
        }
        public virtual void MoveChunk(int3 from, int3 to)
        {
            if (from.Equals(to)) { return; }

            if (DoesChunkExistAtCoordinate(to)) { return; }

            if (TryGetDataChunk(from, out T existingData))
            {
                RemoveChunkUnchecked(from);
                GenerateDataForChunkUnchecked(to, existingData);
            }
        }
        public void RemoveChunkUnchecked(int3 chunkCoordinate)
        {
            _chunks.Remove(chunkCoordinate);
        }
        public void GenerateDataForChunk(int3 chunkCoordinate)
        {
            if (!DoesChunkExistAtCoordinate(chunkCoordinate))
            {
                GenerateDataForChunkUnchecked(chunkCoordinate);
            }
        }
        public void GenerateDataForChunk(int3 chunkCoordinate, T existingData)
        {
            if (!DoesChunkExistAtCoordinate(chunkCoordinate))
            {
                GenerateDataForChunkUnchecked(chunkCoordinate, existingData);
            }
        }
        public abstract void GenerateDataForChunkUnchecked(int3 chunkCoordinate);
        public abstract void GenerateDataForChunkUnchecked(int3 chunkCoordinate, T existingData);
        public void AddChunk(int3 chunkCoordinate, T data)
        {
            if (!DoesChunkExistAtCoordinate(chunkCoordinate))
            {
                AddChunkUnchecked(chunkCoordinate, data);
            }
        }
        public void AddChunkUnchecked(int3 chunkCoordinate, T data)
        {
            _chunks.Add(chunkCoordinate, data);
        }
        public virtual IEnumerable<int3> GetChunkCoordinatesOutsideOfRange(int3 coordinate, int range)
        {
            foreach (int3 chunkCoordinate in _chunks.Keys.ToList())
            {
                if(DistanceUtilities.ChebyshevDistanceGreaterThan(coordinate, chunkCoordinate, range))
                {
                    yield return chunkCoordinate;
                }
            }
        }
        public void ForEachVoxelDataArrayInQuery(BoundsInt worldSpaceQuery, Action<int3, T> function)
        {
            int3 chunkSize = VoxelWorld.WorldSettings.ChunkSize;

            int3 minChunkCoordinate = VectorUtilities.WorldPositionToCoordinate(worldSpaceQuery.min - Vector3Int.one, chunkSize);
            int3 maxChunkCoordinate = VectorUtilities.WorldPositionToCoordinate(worldSpaceQuery.max, chunkSize);

            for (int chunkCoordinateX = minChunkCoordinate.x; chunkCoordinateX <= maxChunkCoordinate.x; chunkCoordinateX++)
            {
                for (int chunkCoordinateY = minChunkCoordinate.y; chunkCoordinateY <= maxChunkCoordinate.y; chunkCoordinateY++)
                {
                    for (int chunkCoordinateZ = minChunkCoordinate.z; chunkCoordinateZ <= maxChunkCoordinate.z; chunkCoordinateZ++)
                    {
                        int3 chunkCoordinate = new int3(chunkCoordinateX, chunkCoordinateY, chunkCoordinateZ);
                        if (TryGetDataChunk(chunkCoordinate, out T voxelDataChunk))
                        {
                            function(chunkCoordinate, voxelDataChunk);
                        }
                    }
                }
            }
        }
    }
}