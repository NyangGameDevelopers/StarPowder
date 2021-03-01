using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Utilities
{
    public static class VectorUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 FloorToMultipleOfX(this float3 n, int3 x)
        {
            return (int3)(math.floor(n / x) * x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 FloorToMultipleOfX(this Vector3 n, int3 x)
        {
            return (int3)(math.floor(new float3(n.x / x.x, n.y / x.y, n.z / x.z)) * x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVectorInt(this int3 n)
        {
            return new Vector3Int(n.x, n.y, n.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 ToInt3(this Vector3 n)
        {
            return new int3((int)n.x, (int)n.y, (int)n.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 ToInt3(this Vector3Int n)
        {
            return new int3(n.x, n.y, n.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 Mod(this int3 n, int3 x)
        {
            return (n % x + x) % x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 WorldPositionToCoordinate(float3 worldPosition, int3 chunkSize)
        {
            return worldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 WorldPositionToCoordinate(Vector3 worldPosition, int3 chunkSize)
        {
            return worldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
        }
    }
}