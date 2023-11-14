namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsResourceVariants
	{
		Task<IResourceVariantData> GetVariantWhenAvailable(int variantIDHash);

		Task<IResourceVariantData> GetVariantWhenAvailable(string variantID);
	}
}