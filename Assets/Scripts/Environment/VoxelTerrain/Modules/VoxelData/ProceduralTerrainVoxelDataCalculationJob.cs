using System.Runtime.CompilerServices;
using LacoLico.VoxelTerrain.Settings;
using Unity.Burst;
using Unity.Mathematics;

namespace LacoLico.VoxelTerrain.VoxelData
{
    [BurstCompile]
    public struct ProceduralTerrainVoxelDataCalculationJob : IVoxelDataGenerationJob
    {
        public ProceduralTerrainSettings ProceduralTerrainSettings { get; set; }
        public int3 WorldPositionOffset { get; set; }
        public VoxelDataVolume<byte> OutputVoxelData { get; set; }
        public void Execute()
        {
            for (int x = 0; x < OutputVoxelData.Width; x++)
            {
                for (int z = 0; z < OutputVoxelData.Depth; z++)
                {
                    int2 terrainPosition = new int2(x + WorldPositionOffset.x, z + WorldPositionOffset.z);
                    float terrainNoise = OctaveNoise(terrainPosition.x, terrainPosition.y, ProceduralTerrainSettings.NoiseFrequency * 0.001f, ProceduralTerrainSettings.NoiseOctaveCount, ProceduralTerrainSettings.NoiseSeed) * ProceduralTerrainSettings.Amplitude;

                    for (int y = 0; y < OutputVoxelData.Height; y++)
                    {
                        int3 worldPosition = new int3(terrainPosition.x, y + WorldPositionOffset.y, terrainPosition.y);

                        float voxelData = (worldPosition.y - ProceduralTerrainSettings.HeightOffset - terrainNoise) * 0.5f;
                        OutputVoxelData.SetVoxelData((byte)math.clamp(voxelData * 255, 0, 255), new int3(x, y, z));
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float OctaveNoise(float x, float y, float frequency, int octaveCount, int seed)
        {
            float value = 0;

            for (int i = 0; i < octaveCount; i++)
            {
                int octaveModifier = (int)math.pow(2, i);
                float pureNoise = (noise.snoise(new float3(octaveModifier * x * frequency, octaveModifier * y * frequency, seed)) + 1) / 2f;
                value += pureNoise / octaveModifier;
            }

            return value;
        }
    }
}