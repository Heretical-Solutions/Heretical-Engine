using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public class NonAllocBroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcastersRepository;

        private readonly ILoggerResolver loggerResolver;

        public NonAllocBroadcasterWithRepositoryBuilder(
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            broadcastersRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        public NonAllocBroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcastersRepository.Add(
                typeof(TBroadcaster),
                DelegatesFactory.BuildNonAllocBroadcasterGeneric<TBroadcaster>(loggerResolver));

            return this;
        }

        public NonAllocBroadcasterWithRepository Build()
        {
            return DelegatesFactory.BuildNonAllocBroadcasterWithRepository(
                broadcastersRepository,
                loggerResolver);
        }
    }
}