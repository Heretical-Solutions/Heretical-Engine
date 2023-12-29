using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.AssetImport.Factories
{
	public static class AssetImporterFactory
	{
		public static AssetImportManager BuildAssetImportManager(
			ApplicationContext context)
		{
			return new AssetImportManager(
				context,
				(IRepository<Type, List<AAssetImportPostProcessor>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, List<AAssetImportPostProcessor>>(),
				(IRepository<Type, INonAllocDecoratedPool<AAssetImporter>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, INonAllocDecoratedPool<AAssetImporter>>());
		}

		public static ConcurrentAssetImportManager BuildConcurrentAssetImportManager(
			ApplicationContext context)
		{
			return new ConcurrentAssetImportManager(
				context,
				(IRepository<Type, List<AAssetImportPostProcessor>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, List<AAssetImportPostProcessor>>(),
				(IRepository<Type, INonAllocDecoratedPool<AAssetImporter>>)
					RepositoriesFactory.BuildDictionaryRepository<Type, INonAllocDecoratedPool<AAssetImporter>>(),
				new SemaphoreSlim(1, 1),
				new SemaphoreSlim(1, 1));
		}
	}
}