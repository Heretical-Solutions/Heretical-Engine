namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents an interface for a read-only runtime resource manager.
    /// </summary>
    public interface IReadOnlyRuntimeResourceManager
    {
        bool HasRootResource(int resourceIDHash);

        bool HasRootResource(string resourceID);

        bool HasResource(int[] resourceIDHashes);

        bool HasResource(string[] resourceIDs);

        IReadOnlyResourceData GetRootResource(int resourceIDHash);

        IReadOnlyResourceData GetRootResource(string resourceID);

        IReadOnlyResourceData GetResource(int[] resourceIDHashes);

        IReadOnlyResourceData GetResource(string[] resourceIDs);

        IResourceVariantData GetDefaultRootResource(int resourceIDHash);

        IResourceVariantData GetDefaultRootResource(string resourceID);

        IResourceVariantData GetDefaultResource(int[] resourceIDHashes);

        IResourceVariantData GetDefaultResource(string[] resourceIDs);

        IEnumerable<int> RootResourceIDHashes { get; }

        IEnumerable<string> RootResourceIDs { get; }

        IEnumerable<IReadOnlyResourceData> AllRootResources { get; }
    }
}