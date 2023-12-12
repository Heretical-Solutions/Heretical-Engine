using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public interface IAssetImportManager
		: IAssetImporterPool
	{
		Task<IResourceVariantData> Import<TImporter>(
			Action<TImporter> initializationDelegate = null,
			IProgress<float> progress = null)
			where TImporter : AAssetImporter;

		Task RegisterPostProcessor<TImporter, TPostProcessor>(
			TPostProcessor instance)
			where TImporter : AAssetImporter
			where TPostProcessor : AAssetImportPostProcessor;
	}
}