using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Utilities.Intersection
{
    public static class IntersectionUtilities
    {
        public static PlaneLineIntersectionResult PlaneLineIntersection(float3 planeOrigin, float3 planeNormal, float3 lineOrigin,
            float3 lineDirection, out float3 intersectionPoint)
        {
            planeNormal = math.normalize(planeNormal);
            lineDirection = math.normalize(lineDirection);

            if (math.dot(planeNormal, lineDirection) == 0)
            {
                intersectionPoint = float3.zero;
                return (planeOrigin - lineOrigin).Equals(float3.zero) ? PlaneLineIntersectionResult.ParallelInsidePlane : PlaneLineIntersectionResult.NoHit;
            }

            float d = math.dot(planeOrigin, -planeNormal);
            float t = -(d + lineOrigin.z * planeNormal.z + lineOrigin.y * planeNormal.y + lineOrigin.x * planeNormal.x) / (lineDirection.z * planeNormal.z + lineDirection.y * planeNormal.y + lineDirection.x * planeNormal.x);
            intersectionPoint = lineOrigin + t * lineDirection;
            return PlaneLineIntersectionResult.OneHit;
        }
        public static BoundsInt GetIntersectionVolume(BoundsInt a, BoundsInt b)
        {
            int3 min = math.max(a.min.ToInt3(), b.min.ToInt3());
            int3 max = math.min(a.max.ToInt3(), b.max.ToInt3());

            int3 size = max - min;
            return new BoundsInt(min.ToVectorInt(), size.ToVectorInt());
        }
    }
}
