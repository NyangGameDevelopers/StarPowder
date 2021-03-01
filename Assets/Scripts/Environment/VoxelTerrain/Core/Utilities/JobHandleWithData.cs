using Unity.Jobs;

namespace LacoLico.VoxelTerrain.Utilities
{
    public class JobHandleWithData<T>
    {
        public JobHandle JobHandle { get; set; }
        public T JobData { get; set; }
    }
}