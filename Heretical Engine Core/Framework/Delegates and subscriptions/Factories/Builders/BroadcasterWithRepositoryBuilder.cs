using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public class BroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcastersRepository;

        private readonly ILoggerResolver loggerResolver;

        public BroadcasterWithRepositoryBuilder(
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            broadcastersRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        public BroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcastersRepository.Add(
                typeof(TBroadcaster),
                DelegatesFactory.BuildBroadcasterGeneric<TBroadcaster>(loggerResolver));

            return this;
        }

        public BroadcasterWithRepository Build()
        {
            return DelegatesFactory.BuildBroadcasterWithRepository(
                broadcastersRepository,
                loggerResolver);
        }
    }
}