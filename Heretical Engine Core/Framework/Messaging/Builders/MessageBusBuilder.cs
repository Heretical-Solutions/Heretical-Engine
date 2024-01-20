using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.Factories
{
    public class MessageBusBuilder
    {
        private readonly IObjectRepository messagePoolRepository;

        private readonly BroadcasterWithRepositoryBuilder broadcasterBuilder;

        private readonly ILoggerResolver loggerResolver;

        public MessageBusBuilder(
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();

            broadcasterBuilder = new BroadcasterWithRepositoryBuilder(
                loggerResolver);
        }

        public MessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate = AllocationsFactory.ActivatorAllocationDelegate<IMessage, TMessage>;

            var initialAllocationCommand = new AllocationCommand<IMessage>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            var additionalAllocationCommand = new AllocationCommand<IMessage>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            IPool<IMessage> messagePool = PoolsFactory.BuildStackPool<IMessage>(
                initialAllocationCommand,
                additionalAllocationCommand,
                loggerResolver);
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public MessageBus Build()
        {
            ILogger logger = 
                loggerResolver?.GetLogger<MessageBus>()
                ?? null;

            return new MessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                new Queue<IMessage>(),
                logger);
        }
    }
}