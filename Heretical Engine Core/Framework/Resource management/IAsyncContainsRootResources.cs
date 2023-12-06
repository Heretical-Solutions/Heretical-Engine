namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsRootResources
	{
		#region Get

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(int rootResourceIDHash);

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(string rootResourceID);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(int[] resourcePathPartHashes);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(string[] resourcePathParts);

		#endregion

		#region Get default

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(int rootResourceIDHash);

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(string rootResourceID);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(int[] resourcePathPartHashes);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(string[] resourcePathParts);

		#endregion
	}
}