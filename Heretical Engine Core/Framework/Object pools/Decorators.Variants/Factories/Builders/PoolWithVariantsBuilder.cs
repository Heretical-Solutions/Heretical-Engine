using HereticalSolutions.RandomGeneration;
using HereticalSolutions.RandomGeneration.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public class PoolWithVariantsBuilder<T>
    {
        private readonly ILoggerResolver loggerResolver;

        private readonly ILogger logger;

        private IRepository<int, VariantContainer<T>> repository;

        private IRandomGenerator random;

        public PoolWithVariantsBuilder(
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
        {
            this.loggerResolver = loggerResolver;

            this.logger = logger;
        }

        public void Initialize()
        {
            repository = RepositoriesFactory.BuildDictionaryRepository<int, VariantContainer<T>>();

            random = RandomFactory.BuildSystemRandomGenerator();
        }

        public void AddVariant(
            int index,
            float chance,
            INonAllocDecoratedPool<T> poolByVariant)
        {
            repository.Add(
                index,
                new VariantContainer<T>
                {
                    Chance = chance,

                    Pool = poolByVariant
                });
        }

        public INonAllocDecoratedPool<T> Build()
        {
            if (repository == null)
                throw new Exception(
                    logger.TryFormat<PoolWithVariantsBuilder<T>>(
                        "BUILDER NOT INITIALIZED"));

            var result = VariantsDecoratorsPoolsFactory.BuildNonAllocPoolWithVariants<T>(
                repository,
                random,
                loggerResolver);

            repository = null;

            random = null;

            return result;
        }
    }
}