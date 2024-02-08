namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents an interface for a read-only runtime resource manager
    /// </summary>
    public interface IReadOnlyRuntimeResourceManager
    {
        #region Has

        bool HasRootResource(int rootResourceIDHash);

        bool HasRootResource(string rootResourceID);

        bool HasResource(int[] resourcePathPartHashes);

        bool HasResource(string[] resourcePathParts);

        #endregion

        #region Get

        IReadOnlyResourceData GetRootResource(int rootResourceIDHash);

        IReadOnlyResourceData GetRootResource(string rootResourceID);

        IReadOnlyResourceData GetResource(int[] resourcePathPartHashes);

        IReadOnlyResourceData GetResource(string[] resourcePathParts);

        #endregion

        #region Try get

        bool TryGetRootResource(
            int rootResourceIDHash,
            out IReadOnlyResourceData resource);

        bool TryGetRootResource(
            string rootResourceID,
            out IReadOnlyResourceData resource);

        bool TryGetResource(
            int[] resourcePathPartHashes,
            out IReadOnlyResourceData resource);

        bool TryGetResource(
            string[] resourcePathParts,
            out IReadOnlyResourceData resource);

        #endregion

        #region Get default

        IResourceVariantData GetDefaultRootResource(int rootResourceIDHash);

        IResourceVariantData GetDefaultRootResource(string rootResourceID);

        IResourceVariantData GetDefaultResource(int[] resourcePathPartHashes);

        IResourceVariantData GetDefaultResource(string[] resourcePathParts);

        #endregion

        #region Try get default

        bool TryGetDefaultRootResource(
            int rootResourceIDHash,
            out IResourceVariantData resource);

        bool TryGetDefaultRootResource(
            string rootResourceID,
            out IResourceVariantData resource);

        bool TryGetDefaultResource(
            int[] resourcePathPartHashes,
            out IResourceVariantData resource);

        bool TryGetDefaultResource(
            string[] resourcePathParts,
            out IResourceVariantData resource);

        #endregion

        #region All's

        IEnumerable<int> RootResourceIDHashes { get; }

        IEnumerable<string> RootResourceIDs { get; }

        IEnumerable<IReadOnlyResourceData> AllRootResources { get; }

        #endregion
    }
}