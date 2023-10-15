using System;

using HereticalSolutions.Collections;

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
    /// Builder class for creating a non-allocating message bus.
    /// </summary>
    public class NonAllocMessageBusBuilder
    {
        private readonly IObjectRepository messagePoolRepository;

        private readonly NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NonAllocMessageBusBuilder"/> class.
        /// </summary>
        /// <param name="logger">The logger instance to use for logging.</param>
        public NonAllocMessageBusBuilder(
            ISmartLogger logger)
        {
            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();

            broadcasterBuilder = new NonAllocBroadcasterWithRepositoryBuilder(logger);
        }

        /// <summary>
        /// Adds a message type to the message bus builder.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to add.</typeparam>
        /// <returns>A reference to the message bus builder.</returns>
        public NonAllocMessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate = AllocationsFactory.ActivatorAllocationDelegate<IMessage, TMessage>;
            
            INonAllocDecoratedPool<IMessage> messagePool = PoolsFactory.BuildResizableNonAllocPool<IMessage>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                });
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        /// <summary>
        /// Builds a non-allocating message bus.
        /// </summary>
        /// <returns>The created non-allocating message bus.</returns>
        public NonAllocMessageBus Build()
        {
            Func<IPoolElement<IMessage>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IPoolElement<IMessage>>;
            
            INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox = PoolsFactory.BuildResizableNonAllocPool<IPoolElement<IMessage>>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                });
            
            var mailboxContents = ((IModifiable<INonAllocPool<IPoolElement<IMessage>>>)mailbox).Contents;
            
            var mailboxContentAsIndexable = (IIndexable<IPoolElement<IPoolElement<IMessage>>>)mailboxContents;
            
            return new NonAllocMessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                mailbox,
                mailboxContentAsIndexable);
        }
    }
}