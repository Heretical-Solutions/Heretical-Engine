using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.AssetImport.Factories
{
	public static class AssetImporterFactory
	{
		public static AssetImportManager BuildAssetImportManager(
			IFormatLogger logger = null)
		{
			return new AssetImportManager(
				(IRepository<Type, List<AAssetImportPostProcessor>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, List<AAssetImportPostProcessor>>(),
				(IRepository<Type, INonAllocDecoratedPool<AAssetImporter>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, INonAllocDecoratedPool<AAssetImporter>>(),
				logger);
		}

		public static ConcurrentAssetImportManager BuildConcurrentAssetImportManager(
			IFormatLogger logger = null)
		{
			return new ConcurrentAssetImportManager(
				(IRepository<Type, List<AAssetImportPostProcessor>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, List<AAssetImportPostProcessor>>(),
				(IRepository<Type, INonAllocDecoratedPool<AAssetImporter>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, INonAllocDecoratedPool<AAssetImporter>>(),
				new SemaphoreSlim(1, 1),
				new SemaphoreSlim(1, 1),
				logger);
		}
	}
}