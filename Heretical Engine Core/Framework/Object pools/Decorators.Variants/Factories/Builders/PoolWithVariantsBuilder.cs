
using System;

using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.RandomGeneration;
using HereticalSolutions.RandomGeneration.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Builder class for creating a pool with variants.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public class PoolWithVariantsBuilder<T>
    {
        private IRepository<int, VariantContainer<T>> repository;

        private IRandomGenerator random;

        /// <summary>
        /// Initializes the builder by creating the repository and random generator.
        /// </summary>
        public void Initialize()
        {
            repository = RepositoriesFactory.BuildDictionaryRepository<int, VariantContainer<T>>();

            random = RandomFactory.BuildSystemRandomGenerator();
        }

        /// <summary>
        /// Adds a variant to the pool given its index, chance, and pool.
        /// </summary>
        /// <param name="index">The index of the variant.</param>
        /// <param name="chance">The chance of the variant being selected.</param>
        /// <param name="poolByVariant">The pool specific to the variant.</param>
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


        /// <summary>
        /// Builds and returns the pool with variants.
        /// </summary>
        /// <returns>The built pool with variants.</returns>
        public INonAllocDecoratedPool<T> Build()
        {
            if (repository == null)
                throw new Exception("[PoolWithVariantsBuilder] BUILDER NOT INITIALIZED");

            var result = VariantsDecoratorsPoolsFactory.BuildNonAllocPoolWithVariants<T>(
                repository,
                random);

            repository = null;

            random = null;

            return result;
        }
    }
}