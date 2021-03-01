using LacoLico.VoxelTerrain.Settings;
using LacoLico.VoxelTerrain.Utilities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace LacoLico.VoxelTerrain.VoxelData
{
    public class ProceduralVoxelDataGenerator : VoxelDataGenerator
    {
        [SerializeField] private ProceduralTerrainSettings proceduralTerrainSettings = new ProceduralTerrainSettings(1, 9, 120, 0, 0);

        public override JobHandleWithData<IVoxelDataGenerationJob> GenerateVoxelData(int3 worldSpaceOrigin, VoxelDataVolume<byte> outputVoxelDataArray)
        {
            ProceduralTerrainVoxelDataCalculationJob job = new ProceduralTerrainVoxelDataCalculationJob
            {
                WorldPositionOffset = worldSpaceOrigin,
                OutputVoxelData = outputVoxelDataArray,
                ProceduralTerrainSettings = proceduralTerrainSettings
            };
            
            JobHandle jobHandle = job.Schedule();

            JobHandleWithData<IVoxelDataGenerationJob> jobHandleWithData = new JobHandleWithData<IVoxelDataGenerationJob>
            {
                JobHandle = jobHandle,
                JobData = job
            };

            return jobHandleWithData;
        }
    }
}