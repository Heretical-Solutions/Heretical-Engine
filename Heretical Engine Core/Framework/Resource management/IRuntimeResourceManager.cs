namespace HereticalSolutions.ResourceManagement
{
    public interface IRuntimeResourceManager
        : IReadOnlyRuntimeResourceManager
    {
        Task AddRootResource(
            IReadOnlyResourceData resource,
            IProgress<float> progress = null);

        Task RemoveRootResource(
            int idHash = -1,
            bool free = true,
            IProgress<float> progress = null);

        Task RemoveRootResource(
            string resourceID,
            bool free = true,
            IProgress<float> progress = null);

        Task ClearAllRootResources(
            bool free = true,
            IProgress<float> progress = null);
    }
}