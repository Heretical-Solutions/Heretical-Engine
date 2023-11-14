namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsNestedResources
	{
		Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(int nestedResourceIDHash);

		Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(string nestedResourceID);
	}
}