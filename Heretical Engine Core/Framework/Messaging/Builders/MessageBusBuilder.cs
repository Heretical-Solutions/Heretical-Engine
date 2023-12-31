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

        private readonly IFormatLogger logger;

        public MessageBusBuilder(
            IFormatLogger logger)
        {
            this.logger = logger;

            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();

            broadcasterBuilder = new BroadcasterWithRepositoryBuilder(
                logger);
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
                logger);
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public MessageBus Build()
        {
            return new MessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                new Queue<IMessage>(),
                logger);
        }
    }
}