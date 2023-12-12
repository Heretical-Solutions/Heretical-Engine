using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public abstract class AAssetImportPostProcessor
	{
		public virtual async Task<IResourceVariantData> OnImport(
			IResourceVariantData variantData,
			IProgress<float> progress = null)
		{
			throw new NotImplementedException();
		}
	}
}