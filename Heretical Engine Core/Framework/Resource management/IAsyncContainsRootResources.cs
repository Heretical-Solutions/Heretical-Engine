namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsRootResources
	{
		#region Get

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(int resourceIDHash);

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(string resourceID);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(int[] resourceIDHashes);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(string[] resourceIDs);

		#endregion

		#region Get default

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(int resourceIDHash);

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(string resourceID);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(int[] resourceIDHashes);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(string[] resourceIDs);

		#endregion
	}
}