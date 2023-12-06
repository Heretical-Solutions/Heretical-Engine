namespace HereticalSolutions.ResourceManagement
{
    public interface IRuntimeResourceManager
        : IReadOnlyRuntimeResourceManager
    {
        Task AddRootResource(
            IReadOnlyResourceData rootResource,
            IProgress<float> progress = null);

        Task RemoveRootResource(
            int rootResourceIDHash = -1,
            bool free = true,
            IProgress<float> progress = null);

        Task RemoveRootResource(
            string rootResourceID,
            bool free = true,
            IProgress<float> progress = null);

        Task ClearAllRootResources(
            bool free = true,
            IProgress<float> progress = null);
    }
}