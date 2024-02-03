namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsDependencyResourceVariants
	{
		Task<IResourceVariantData> GetDependencyResourceVariant(string variantID = null);
	}
}