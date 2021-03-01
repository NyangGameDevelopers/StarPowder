using System;
using LacoLico.VoxelTerrain.Utilities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.VoxelData
{
    public struct VoxelDataVolume<T> : IDisposable where T : struct
    {
        private NativeArray<T> _voxelData;
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }
        public int3 Size => new int3(Width, Height, Depth);
        public int Length => Width * Height * Depth;
        public bool IsCreated => _voxelData.IsCreated;
        public VoxelDataVolume(int width, int height, int depth) : this(width, height, depth, Allocator.Persistent) { }
        public VoxelDataVolume(int width, int height, int depth, Allocator allocator)
        {
            if (width < 0 || height < 0 || depth < 0)
            {
                throw new ArgumentException("The dimensions of this volume must all be positive!");
            }

            _voxelData = new NativeArray<T>(width * height * depth, allocator);

            Width = width;
            Height = height;
            Depth = depth;
        }
        public VoxelDataVolume(int size) : this(size, Allocator.Persistent) { }
        public VoxelDataVolume(int size, Allocator allocator) : this(size, size, size, allocator) { }
        public VoxelDataVolume(int3 size) : this(size, Allocator.Persistent) { }
        public VoxelDataVolume(int3 size, Allocator allocator) : this(size.x, size.y, size.z, allocator) { }
        public VoxelDataVolume(Vector3Int size) : this(size, Allocator.Persistent) { }
        public VoxelDataVolume(Vector3Int size, Allocator allocator) : this(size.ToInt3(), allocator) { }
        public void Dispose()
        {
            _voxelData.Dispose();
        }
        public void SetVoxelData(T voxelData, int3 localPosition)
        {
            int index = IndexUtilities.XyzToIndex(localPosition, Width, Height);
            SetVoxelData(voxelData, index);
        }
        public void SetVoxelData(T voxelData, int x, int y, int z)
        {
            int index = IndexUtilities.XyzToIndex(x, y, z, Width, Height);
            SetVoxelData(voxelData, index);
        }
        public void SetVoxelData(T voxelData, int index)
        {
            _voxelData[index] = voxelData;
        }
        public bool TryGetVoxelData(int3 localPosition, out T voxelData)
        {
            return TryGetVoxelData(localPosition.x, localPosition.y, localPosition.z, out voxelData);
        }
        public bool TryGetVoxelData(int x, int y, int z, out T voxelData)
        {
            int index = IndexUtilities.XyzToIndex(x, y, z, Width, Height);
            return TryGetVoxelData(index, out voxelData);
        }
        public bool TryGetVoxelData(int index, out T voxelData)
        {
            if (index >= 0 && index < _voxelData.Length)
            {
                voxelData = GetVoxelData(index);
                return true;
            }

            voxelData = default;
            return false;
        }
        public T GetVoxelData(int3 localPosition)
        {
            return GetVoxelData(localPosition.x, localPosition.y, localPosition.z);
        }
        public T GetVoxelData(int x, int y, int z)
        {
            int index = IndexUtilities.XyzToIndex(x, y, z, Width, Height);
            return GetVoxelData(index);
        }
        public T GetVoxelData(int index)
        {
            return _voxelData[index];
        }
        public void CopyFrom(VoxelDataVolume<T> sourceVolume)
        {
            if (Width == sourceVolume.Width && Height == sourceVolume.Height && Depth == sourceVolume.Depth)
            {
                _voxelData.CopyFrom(sourceVolume._voxelData);
            }
            else
            {
                throw new ArgumentException($"The chunks are not the same size! Width: {Width}/{sourceVolume.Width}, Height: {Height}/{sourceVolume.Height}, Depth: {Depth}/{sourceVolume.Depth}");
            }
        }
        public int GetIndex(int3 voxelDataLocalPosition)
        {
            return IndexUtilities.XyzToIndex(voxelDataLocalPosition, Width, Height);
        }
        public unsafe void* GetUnsafePtr()
        {
            return _voxelData.GetUnsafePtr();
        }
    }
}