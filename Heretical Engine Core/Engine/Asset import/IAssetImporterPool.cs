using HereticalSolutions.Pools;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public interface IAssetImporterPool
	{
		Task<IPoolElement<AAssetImporter>> PopImporter<TImporter>()
			where TImporter : AAssetImporter;

		Task PushImporter(
			IPoolElement<AAssetImporter> pooledImporter);
	}
}