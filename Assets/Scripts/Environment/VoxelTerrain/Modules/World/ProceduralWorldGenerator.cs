using LacoLico.VoxelTerrain.Chunks;
using LacoLico.VoxelTerrain.Utilities;
using LacoLico.VoxelTerrain.Utilities.Intersection;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.World
{
    public class ProceduralWorldGenerator : MonoBehaviour
    {
        [SerializeField] private VoxelWorld voxelWorld;
        [SerializeField] private ProceduralChunkProvider chunkProvider;
        [SerializeField] private int renderDistance = 5;
        [SerializeField] private int loadingBufferSize = 2;
        [SerializeField] private Transform player;
        private int3 _lastGenerationCoordinate;

        private void Start()
        {
            int3 playerCoordinate = GetPlayerCoordinate();
            GenerateTerrainAroundCoordinate(playerCoordinate);
        }

        private void Update()
        {
            int3 newPlayerCoordinate = GetPlayerCoordinate();
            if (!newPlayerCoordinate.Equals(_lastGenerationCoordinate))
            {
                MoveVoxelData(_lastGenerationCoordinate, newPlayerCoordinate);

                var newlyFreedCoordinates = voxelWorld.ChunkStore.GetChunkCoordinatesOutsideOfRange(newPlayerCoordinate, renderDistance);

                int3 renderSize = new int3(renderDistance * 2 + 1);

                int3 oldPos = _lastGenerationCoordinate - new int3(renderDistance);
                BoundsInt oldCoords = new BoundsInt(oldPos.ToVectorInt(), renderSize.ToVectorInt());

                int3 newPos = newPlayerCoordinate - new int3(renderDistance);
                BoundsInt newCoords = new BoundsInt(newPos.ToVectorInt(), renderSize.ToVectorInt());

                int3[] coordinatesThatNeedChunks = CoordinateUtilities.GetCoordinatesThatNeedChunks(oldCoords, newCoords);

                int i = 0;
                foreach (int3 source in newlyFreedCoordinates)
                {
                    int3 target = coordinatesThatNeedChunks[i];

                    voxelWorld.ChunkStore.MoveChunk(source, target);

                    voxelWorld.VoxelColorStore.MoveChunk(source, target);

                    chunkProvider.AddChunkToGenerationQueue(target);

                    i++;
                }

                _lastGenerationCoordinate = newPlayerCoordinate;
            }
        }
        private void MoveVoxelData(int3 playerFromCoordinate, int3 playerToCoordinate)
        {
            int range = renderDistance + loadingBufferSize;
            int3 renderSize = new int3(range * 2 + 1);

            int3 oldPos = playerFromCoordinate - new int3(range);
            BoundsInt oldCoords = new BoundsInt(oldPos.ToVectorInt(), renderSize.ToVectorInt());

            int3 newPos = playerToCoordinate - new int3(range);
            BoundsInt newCoords = new BoundsInt(newPos.ToVectorInt(), renderSize.ToVectorInt());

            int3[] coordinatesThatNeedData = CoordinateUtilities.GetCoordinatesThatNeedChunks(oldCoords, newCoords);

            var newlyFreedCoordinates = voxelWorld.VoxelDataStore.GetChunkCoordinatesOutsideOfRange(playerToCoordinate, range);

            int i = 0;
            foreach (int3 freeCoordinate in newlyFreedCoordinates)
            {
                var targetCoordinate = coordinatesThatNeedData[i];
                voxelWorld.VoxelDataStore.MoveChunk(freeCoordinate, targetCoordinate);
                i++;
            }
        }
        private int3 GetPlayerCoordinate()
        {
            return VectorUtilities.WorldPositionToCoordinate(player.position, voxelWorld.WorldSettings.ChunkSize);
        }
        private void GenerateTerrainAroundCoordinate(int3 coordinate)
        {
            int3[] preloadCoordinates = CoordinateUtilities.GetPreloadCoordinates(coordinate, renderDistance, loadingBufferSize);
            for (int i = 0; i < preloadCoordinates.Length; i++)
            {
                int3 loadingCoordinate = preloadCoordinates[i];
                voxelWorld.VoxelDataStore.GenerateDataForChunk(loadingCoordinate);
            }

            int3[] chunkGenerationCoordinates = CoordinateUtilities.GetChunkGenerationCoordinates(coordinate, renderDistance);
            for (int i = 0; i < chunkGenerationCoordinates.Length; i++)
            {
                int3 generationCoordinate = chunkGenerationCoordinates[i];
                voxelWorld.VoxelColorStore.GenerateDataForChunk(generationCoordinate);
                chunkProvider.EnsureChunkExistsAtCoordinate(generationCoordinate);
            }

            _lastGenerationCoordinate = coordinate;
        }
    }
}