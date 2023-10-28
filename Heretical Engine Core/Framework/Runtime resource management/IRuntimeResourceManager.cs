namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents an interface for a runtime resource manager.
    /// </summary>
    public interface IRuntimeResourceManager
        : IReadOnlyRuntimeResourceManager
    {
        /// <summary>
        /// Adds a resource to the runtime resource manager.
        /// </summary>
        /// <param name="resource">The read-only resource data to add.</param>
        void AddResource(IReadOnlyResourceData resource);

        /// <summary>
        /// Removes a resource from the runtime resource manager.
        /// </summary>
        /// <param name="idHash">The hash value of the resource ID to remove. If not specified, all resources will be removed.</param>
        void RemoveResource(int idHash = -1);

        /// <summary>
        /// Removes a resource from the runtime resource manager.
        /// </summary>
        /// <param name="resourceID">The ID of the resource to remove.</param>
        void RemoveResource(string resourceID);
    }
}