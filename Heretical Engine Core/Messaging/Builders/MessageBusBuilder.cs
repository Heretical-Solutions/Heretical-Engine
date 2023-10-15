using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Logging;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Messaging.Factories
{
    /// <summary>
    /// Builder class for creating a message bus.
    /// </summary>
    public class MessageBusBuilder
    {
        private readonly IObjectRepository messagePoolRepository;
        private readonly BroadcasterWithRepositoryBuilder broadcasterBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusBuilder"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used.</param>
        public MessageBusBuilder(
            ISmartLogger logger)
        {
            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();
            broadcasterBuilder = new BroadcasterWithRepositoryBuilder(logger);
        }

        /// <summary>
        /// Adds a message type to the message bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of message.</typeparam>
        /// <returns>The instance of the <see cref="MessageBusBuilder"/>.</returns>
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
                additionalAllocationCommand);
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        /// <summary>
        /// Builds an instance of the <see cref="MessageBus"/> class.
        /// </summary>
        /// <returns>An instance of the <see cref="MessageBus"/> class.</returns>
        public MessageBus Build()
        {
            return new MessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                new Queue<IMessage>());
        }
    }
}