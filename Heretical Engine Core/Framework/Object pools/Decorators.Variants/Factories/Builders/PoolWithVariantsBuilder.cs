using HereticalSolutions.RandomGeneration;
using HereticalSolutions.RandomGeneration.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public class PoolWithVariantsBuilder<T>
    {
        private readonly IFormatLogger logger;

        private IRepository<int, VariantContainer<T>> repository;

        private IRandomGenerator random;

        public PoolWithVariantsBuilder(IFormatLogger logger)
        {
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
                logger?.ThrowException<PoolWithVariantsBuilder<T>>(
                    "BUILDER NOT INITIALIZED");

            var result = VariantsDecoratorsPoolsFactory.BuildNonAllocPoolWithVariants<T>(
                repository,
                random,
                logger);

            repository = null;

            random = null;

            return result;
        }
    }
}