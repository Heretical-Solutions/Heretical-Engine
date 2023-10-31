using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.Broadcasting;
using HereticalSolutions.Logging;
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Factories
{
    /// <summary>
    /// Represents a builder class for creating a BroadcasterWithRepository object.
    /// </summary>
    public class BroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcastersRepository;

        private readonly IFormatLogger logger;

        /// <summary>
        /// Initializes a new instance of the BroadcasterWithRepositoryBuilder class.
        /// </summary>
        /// <param name="logger">The logger to be used.</param>
        public BroadcasterWithRepositoryBuilder(
            IFormatLogger logger)
        {
            this.logger = logger;
            
            broadcastersRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        /// <summary>
        /// Adds a broadcaster of type TBroadcaster to the repository.
        /// </summary>
        /// <typeparam name="TBroadcaster">The type of the broadcaster.</typeparam>
        /// <returns>The current instance of the BroadcasterWithRepositoryBuilder class.</returns>
        public BroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcastersRepository.Add(typeof(TBroadcaster), DelegatesFactory.BuildBroadcasterGeneric<TBroadcaster>());

            return this;
        }

        /// <summary>
        /// Builds a BroadcasterWithRepository object with the provided repository and logger.
        /// </summary>
        /// <returns>A new instance of the BroadcasterWithRepository class.</returns>
        public BroadcasterWithRepository Build()
        {
            return DelegatesFactory.BuildBroadcasterWithRepository(
                broadcastersRepository,
                logger);
        }
    }
}