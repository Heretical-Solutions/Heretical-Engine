namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents an interface for a read-only runtime resource manager.
    /// </summary>
    public interface IReadOnlyRuntimeResourceManager
    {
        /// <summary>
        /// Checks if a resource with the specified resource ID hash exists in the manager.
        /// </summary>
        /// <param name="resourceIDHash">The hash value of the resource ID.</param>
        /// <returns>True if the resource exists, false otherwise.</returns>
        bool HasResource(int resourceIDHash);

        /// <summary>
        /// Checks if a resource with the specified resource ID exists in the manager.
        /// </summary>
        /// <param name="resourceID">The ID of the resource.</param>
        /// <returns>True if the resource exists, false otherwise.</returns>
        bool HasResource(string resourceID);

        /// <summary>
        /// Gets the resource with the specified resource ID hash.
        /// </summary>
        /// <param name="resourceIDHash">The hash value of the resource ID.</param>
        /// <returns>The read-only resource data associated with the resource ID hash, or null if not found.</returns>
        IReadOnlyResourceData GetResource(int resourceIDHash);

        /// <summary>
        /// Gets the resource with the specified resource ID.
        /// </summary>
        /// <param name="resourceID">The ID of the resource.</param>
        /// <returns>The read-only resource data associated with the resource ID, or null if not found.</returns>
        IReadOnlyResourceData GetResource(string resourceID);

        /// <summary>
        /// Gets the default resource storage handle for the resource with the specified resource ID hash.
        /// </summary>
        /// <param name="resourceIDHash">The hash value of the resource ID.</param>
        /// <returns>The default resource storage handle associated with the resource ID hash.</returns>
        IResourceStorageHandle GetDefaultResource(int resourceIDHash);

        /// <summary>
        /// Gets the default resource storage handle for the resource with the specified resource ID.
        /// </summary>
        /// <param name="resourceID">The ID of the resource.</param>
        /// <returns>The default resource storage handle associated with the resource ID.</returns>
        IResourceStorageHandle GetDefaultResource(string resourceID);
    }
}