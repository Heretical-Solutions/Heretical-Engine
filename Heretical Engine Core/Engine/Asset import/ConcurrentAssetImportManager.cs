using HereticalSolutions.Allocations;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public class ConcurrentAssetImportManager
		: IAssetImportManager
	{
		private static readonly AllocationCommandDescriptor initialAllocation = new AllocationCommandDescriptor
		{
			Rule = EAllocationAmountRule.ADD_ONE,

			Amount = 1
		};

		private static readonly AllocationCommandDescriptor additionalAllocation = new AllocationCommandDescriptor
		{
			Rule = EAllocationAmountRule.DOUBLE_AMOUNT,

			Amount = 0
		};

		private readonly ApplicationContext context;

		private readonly IRepository<Type, List<AAssetImportPostProcessor>> postProcessorRepository;

		private readonly IRepository<Type, INonAllocDecoratedPool<AAssetImporter>> importerPoolRepository;

		private readonly SemaphoreSlim importerPoolSemaphore;

		private readonly SemaphoreSlim postProcessorsSemaphore;

		public ConcurrentAssetImportManager(
			ApplicationContext context,
			IRepository<Type, List<AAssetImportPostProcessor>> postProcessorRepository,
			IRepository<Type, INonAllocDecoratedPool<AAssetImporter>> importerPoolRepository,
			SemaphoreSlim importerPoolSemaphore,
			SemaphoreSlim postProcessorsSemaphore)
		{
			this.context = context;

			this.postProcessorRepository = postProcessorRepository;

			this.importerPoolRepository = importerPoolRepository;

			this.importerPoolSemaphore = importerPoolSemaphore;

			this.postProcessorsSemaphore = postProcessorsSemaphore;
		}

		#region IAssetImportManager

		public async Task<IResourceVariantData> Import<TImporter>(
			Action<TImporter> initializationDelegate = null,
			IProgress<float> progress = null)
			where TImporter : AAssetImporter
		{
			context.Logger?.Log<ConcurrentAssetImportManager>($"IMPORTING {typeof(TImporter).Name} INITIATED");

			var importer = await PopImporter<TImporter>()
				.ThrowExceptions< IPoolElement<AAssetImporter>, AssetImportManager>(
					context.Logger);

			initializationDelegate?.Invoke(
				importer.Value as TImporter);

			var result = await importer.Value.Import(
				progress)
				.ThrowExceptions<IResourceVariantData, AssetImportManager>(
					context.Logger);

			AAssetImportPostProcessor[] postProcessors = null;

			await postProcessorsSemaphore.WaitAsync();

			try
			{
				if (postProcessorRepository.Has(typeof(TImporter)))
				{
					postProcessors = postProcessorRepository
						.Get(
							typeof(TImporter))
						.ToArray();
				}
			}
			finally
			{
				postProcessorsSemaphore.Release();
			}

			if (postProcessors != null)
			{
				for (int i = 0; i < postProcessors.Length; i++)
				{
					//This one should NOT be paralleled
					//Imagine one post processor adding 10 exrta vertices to the mesh while the other one substracts 20
					//Why? For any fucking reason. PostProcessors are exposed as API and I have zero idea what kind of shit
					//would a user come up with its usage
					await postProcessors[i].OnImport(
						result,
						progress)
						.ThrowExceptions<AssetImportManager>(
							context.Logger);
				}
			}

			await PushImporter(importer);

			context.Logger?.Log<ConcurrentAssetImportManager>($"IMPORTING {typeof(TImporter).Name} FINISHED");

			return result;
		}

		public async Task RegisterPostProcessor<TImporter, TPostProcessor>(
			TPostProcessor instance)
			where TImporter : AAssetImporter
			where TPostProcessor : AAssetImportPostProcessor
		{
			await postProcessorsSemaphore.WaitAsync();

			try
			{
				if (!postProcessorRepository.Has(typeof(TImporter)))
				{
					postProcessorRepository.Add(
						typeof(TImporter),
						new List<AAssetImportPostProcessor>());
				}

				var postProcessors = postProcessorRepository.Get(
					typeof(TImporter));

				postProcessors.Add(
					instance);
			}
			finally
			{
				postProcessorsSemaphore.Release();
			}
		}

		#region IAssetImporterPool

		public async Task<IPoolElement<AAssetImporter>> PopImporter<TImporter>()
			where TImporter : AAssetImporter
		{
			INonAllocDecoratedPool<AAssetImporter> importerPool;

			await importerPoolSemaphore.WaitAsync();

			try
			{
				if (!importerPoolRepository.Has(typeof(TImporter)))
				{
					importerPool = PoolsFactory.BuildSimpleResizableObjectPool<AAssetImporter, TImporter>(
						initialAllocation,
						additionalAllocation,
						context.Logger,
						new object[]
						{
							context
						});

					importerPoolRepository.Add(
						typeof(TImporter),
						importerPool);
				}
				else
				{
					importerPool = importerPoolRepository.Get(
						typeof(TImporter));
				}
			}
			finally
			{
				importerPoolSemaphore.Release();
			}

			return importerPool.Pop(null);
		}

		public async Task PushImporter(
			IPoolElement<AAssetImporter> pooledImporter)
		{
			pooledImporter.Value.Cleanup();

			await importerPoolSemaphore.WaitAsync();

			try
			{
				pooledImporter.Push();
			}
			finally
			{
				importerPoolSemaphore.Release();
			}
		}

		#endregion

		#endregion
	}
}