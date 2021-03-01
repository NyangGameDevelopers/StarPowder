using System;
using UnityEngine;

namespace LacoLico.VoxelTerrain.Settings
{
    [Serializable]
    public struct ProceduralTerrainSettings
    {
        [SerializeField] private float noiseFrequency;
        [SerializeField] private int noiseOctaveCount;
        [SerializeField] private float amplitude;
        [SerializeField] private float heightOffset;
        [SerializeField] private int noiseSeed;
        public float NoiseFrequency { get => noiseFrequency; set => noiseFrequency = value; }
        public int NoiseOctaveCount { get => noiseOctaveCount; set => noiseOctaveCount = value; }
        public float Amplitude { get => amplitude; set => amplitude = value; }
        public float HeightOffset { get => heightOffset; set => heightOffset = value; }
        public int NoiseSeed { get => noiseSeed; set => noiseSeed = value; }
        public ProceduralTerrainSettings(float noiseFrequency, int noiseOctaveCount, float amplitude, float heightOffset, int noiseSeed)
        {
            this.noiseFrequency = noiseFrequency;
            this.noiseOctaveCount = noiseOctaveCount;
            this.amplitude = amplitude;
            this.heightOffset = heightOffset;
            this.noiseSeed = noiseSeed;
        }
    }
}