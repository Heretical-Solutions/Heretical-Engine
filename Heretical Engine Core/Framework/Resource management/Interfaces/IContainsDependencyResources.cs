namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsDependencyResources
	{
		Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,
			IProgress<float> progress = null);

		Task<IReadOnlyResourceData> GetDependencyResource(
			string path);
	}
}