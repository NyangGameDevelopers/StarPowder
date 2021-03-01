using Unity.Mathematics;

namespace LacoLico.VoxelTerrain.Utilities
{
    public static class DistanceUtilities
    {
        public static bool ChebyshevDistanceGreaterThan(int3 pointA, int3 pointB, int maximumAllowed)
        {
            int abx = pointA.x - pointB.x;
            int aby = pointA.y - pointB.y;
            int abz = pointA.z - pointB.z;

            return abx > maximumAllowed || -abx > maximumAllowed || aby > maximumAllowed || -aby > maximumAllowed || abz > maximumAllowed || -abz > maximumAllowed;
        }
    }
}
