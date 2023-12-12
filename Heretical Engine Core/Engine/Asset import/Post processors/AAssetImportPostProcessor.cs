using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public abstract class AAssetImportPostProcessor
	{
		public virtual async Task OnImport(
			IResourceVariantData variantData,
			IProgress<float> progress = null)
		{
			throw new NotImplementedException();
		}
	}
}