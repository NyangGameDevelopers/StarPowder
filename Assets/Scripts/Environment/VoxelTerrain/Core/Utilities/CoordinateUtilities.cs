using LacoLico.VoxelTerrain.Utilities.Intersection;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Utilities
{
    public static class CoordinateUtilities
    {
        public static int3[] GetChunkCoordinatesContainingPoint(int3 worldPosition, int3 chunkSize)
        {
            int3 localPosition = VectorUtilities.Mod(worldPosition, chunkSize);

            int chunkCheckCountX = localPosition.x == 0 ? 1 : 0;
            int chunkCheckCountY = localPosition.y == 0 ? 1 : 0;
            int chunkCheckCountZ = localPosition.z == 0 ? 1 : 0;

            int chunkCount = 1 << (chunkCheckCountX + chunkCheckCountY + chunkCheckCountZ);
            int3[] chunkCoordinates = new int3[chunkCount];

            int3 origin = VectorUtilities.WorldPositionToCoordinate(worldPosition, chunkSize);

            int addedIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                var cornerOffset = LookupTables.CubeCorners[i];
                if (cornerOffset.x <= chunkCheckCountX && cornerOffset.y <= chunkCheckCountY && cornerOffset.z <= chunkCheckCountZ)
                {
                    chunkCoordinates[addedIndex] = origin - cornerOffset;
                    addedIndex++;
                }
            }

            return chunkCoordinates;
        }
        public static int3[] GetPreloadCoordinates(int3 centerCoordinate, int innerSize, int outerSize)
        {
            int3 min = -new int3(innerSize + outerSize);
            int3 max = new int3(innerSize + outerSize);

            int3 innerMin = -new int3(innerSize);
            int3 innerMax = new int3(innerSize);

            int3 fullSize = max - min + 1;
            int fullVolume = fullSize.x * fullSize.y * fullSize.z;

            int3 innerDimensions = innerMax - innerMin + 1;
            int innerVolume = innerDimensions.x * innerDimensions.y * innerDimensions.z;

            int3[] result = new int3[fullVolume - innerVolume];

            int index = 0;
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        if (innerMin.x <= x && x <= innerMax.x &&
                            innerMin.y <= y && y <= innerMax.y &&
                            innerMin.z <= z && z <= innerMax.z)
                        {
                            continue;
                        }

                        result[index] = new int3(x, y, z) + centerCoordinate;
                        index++;
                    }
                }
            }

            return result;
        }
        public static int3[] GetChunkGenerationCoordinates(int3 centerChunkCoordinate, int renderDistance)
        {
            int3[] coordinates = new int3[(int)math.pow(renderDistance * 2 + 1, 3)];
            int i = 0;
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    for (int z = -renderDistance; z <= renderDistance; z++)
                    {
                        int3 chunkCoordinate = centerChunkCoordinate + new int3(x, y, z);
                        coordinates[i] = chunkCoordinate;
                        i++;
                    }
                }
            }

            return coordinates;
        }
        public static int3[] GetCoordinatesThatNeedChunks(BoundsInt oldChunks, BoundsInt newChunks)
        {
            // Cache the min/max values because accessing them repeatedly in a loop is surprisingly costly
            int newChunksMinX = newChunks.xMin;
            int newChunksMaxX = newChunks.xMax;
            int newChunksMinY = newChunks.yMin;
            int newChunksMaxY = newChunks.yMax;
            int newChunksMinZ = newChunks.zMin;
            int newChunksMaxZ = newChunks.zMax;

            int oldChunksMinX = oldChunks.xMin;
            int oldChunksMaxX = oldChunks.xMax;
            int oldChunksMinY = oldChunks.yMin;
            int oldChunksMaxY = oldChunks.yMax;
            int oldChunksMinZ = oldChunks.zMin;
            int oldChunksMaxZ = oldChunks.zMax;

            int count = newChunks.CalculateVolume();

            BoundsInt intersection = IntersectionUtilities.GetIntersectionVolume(oldChunks, newChunks);
            if(math.all(intersection.size.ToInt3() > 0))
            {
                count -= intersection.CalculateVolume();
            }

            int3[] coordinates = new int3[count];

            int i = 0;
            for (int x = newChunksMinX; x < newChunksMaxX; x++)
            {
                for (int y = newChunksMinY; y < newChunksMaxY; y++)
                {
                    for (int z = newChunksMinZ; z < newChunksMaxZ; z++)
                    {
                        if (oldChunksMinX <= x && x < oldChunksMaxX &&
                            oldChunksMinY <= y && y < oldChunksMaxY &&
                            oldChunksMinZ <= z && z < oldChunksMaxZ)
                        {
                            continue;
                        }

                        coordinates[i] = new int3(x, y, z);
                        i++;
                    }
                }
            }

            return coordinates;
        }
    }
}