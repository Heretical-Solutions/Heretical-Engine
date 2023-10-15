using System;

using HereticalSolutions.Delegates.Broadcasting;
using HereticalSolutions.Logging;
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Factories
{
    /// <summary>
    /// Represents a builder for creating a <see cref="NonAllocBroadcasterWithRepository"/>.
    /// </summary>
    public class NonAllocBroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcastersRepository;
        private readonly ISmartLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAllocBroadcasterWithRepositoryBuilder"/> class.
        /// </summary>
        /// <param name="logger">The logger implementation to be used.</param>
        public NonAllocBroadcasterWithRepositoryBuilder(ISmartLogger logger)
        {
            this.logger = logger;

            broadcastersRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        /// <summary>
        /// Adds a broadcaster of type <typeparamref name="TBroadcaster"/> to the repository.
        /// </summary>
        /// <typeparam name="TBroadcaster">The type of the broadcaster to be added.</typeparam>
        /// <returns>The current instance of the <see cref="NonAllocBroadcasterWithRepositoryBuilder"/>.</returns>
        public NonAllocBroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcastersRepository.Add(typeof(TBroadcaster), DelegatesFactory.BuildNonAllocBroadcasterGeneric<TBroadcaster>());

            return this;
        }

        /// <summary>
        /// Builds and returns a <see cref="NonAllocBroadcasterWithRepository"/> using the added broadcasters and logger.
        /// </summary>
        /// <returns>A <see cref="NonAllocBroadcasterWithRepository"/> built with the added broadcasters and logger.</returns>
        public NonAllocBroadcasterWithRepository Build()
        {
            return DelegatesFactory.BuildNonAllocBroadcasterWithRepository(
                broadcastersRepository,
                logger);
        }
    }
}