namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents an interface for a read-only runtime resource manager.
    /// </summary>
    public interface IReadOnlyRuntimeResourceManager
    {
        #region Has

        bool HasRootResource(int resourceIDHash);

        bool HasRootResource(string resourceID);

        bool HasResource(int[] resourceIDHashes);

        bool HasResource(string[] resourceIDs);

        #endregion

        #region Get

        IReadOnlyResourceData GetRootResource(int resourceIDHash);

        IReadOnlyResourceData GetRootResource(string resourceID);

        IReadOnlyResourceData GetResource(int[] resourceIDHashes);

        IReadOnlyResourceData GetResource(string[] resourceIDs);

        #endregion

        #region Try get

        bool TryGetRootResource(
            int resourceIDHash,
            out IReadOnlyResourceData resource);

        bool TryGetRootResource(
            string resourceID,
            out IReadOnlyResourceData resource);

        bool TryGetResource(
            int[] resourceIDHashes,
            out IReadOnlyResourceData resource);

        bool TryGetResource(
            string[] resourceIDs,
            out IReadOnlyResourceData resource);

        #endregion

        #region Get default

        IResourceVariantData GetDefaultRootResource(int resourceIDHash);

        IResourceVariantData GetDefaultRootResource(string resourceID);

        IResourceVariantData GetDefaultResource(int[] resourceIDHashes);

        IResourceVariantData GetDefaultResource(string[] resourceIDs);

        #endregion

        #region Try get default

        bool TryGetDefaultRootResource(
            int resourceIDHash,
            out IResourceVariantData resource);

        bool TryGetDefaultRootResource(
            string resourceID,
            out IResourceVariantData resource);

        bool TryGetDefaultResource(
            int[] resourceIDHashes,
            out IResourceVariantData resource);

        bool TryGetDefaultResource(
            string[] resourceIDs,
            out IResourceVariantData resource);

        #endregion

        #region All's

        IEnumerable<int> RootResourceIDHashes { get; }

        IEnumerable<string> RootResourceIDs { get; }

        IEnumerable<IReadOnlyResourceData> AllRootResources { get; }

        #endregion
    }
}