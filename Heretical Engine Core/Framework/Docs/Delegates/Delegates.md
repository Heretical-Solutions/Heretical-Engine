# [Work in progress]

---

# Delegates

## TL;DR

### Non-alloc reasoning

- C# `delegates` (including `Actions` and `Funcs`) are designed to be `multicasts`, meaning that whenever you store an instance of the delegate, you actually store an entire array of method references.
- When you invoke a delegate, you actually invoke all the method references in the multicast in the `invocation order`.
- Delegate invocations are usually twice as expensive as regular method calls, but that is still cheap compared to other methods (like reflection), so the performance aspect is not that much of a problem.
- The problem is that whenever you add or remove a method reference to the multicast via `+=` or `-=` or even perform a simple comparison to `null`, you allocate a small amount of RAM.
- When such operations add up (and with event-rich architecture or basically any UI framework, you will eventually reach that point), you may get some large numbers of RAM allocations per frame.
- Even if kilobytes to megabytes of RAM allocations per frame are not making a significant impact on desktop platforms, mobile devices usually struggle with it, and as such, there is a demand for allocation-free delegate subscription and invocation methods.

### Non-alloc versions of pub/sub

- Each publisher implements both a version of the `IPublisher` interface and a version of the `ISubscribable` interface with the argument options of choice.
- The `ISubscribable` interface family is designed to allow subscription holders to un/subscribe their subscriptions to the publisher but not to call its publishing methods.
- The `IPublisher` interface family is designed to allow publisher invokers to invoke the publisher but not to call its un/subscribe methods.
- Each subscriber implements an `ISubscription` interface and an arbitrary (but always >= 1) amount of pairs of `ISubscriptionHandler` and `ISubscriptionState` interfaces that share the same `TInvokable` generic type.
- The `ISubscription` interface is inherited by all subscriptions to provide subscription holders with a convenient storage of multiple subscription instances with any amount and type of arguments. The publishers take care of finding out those internally.
- The `ISubscriptionHandler` interface is used by publishers to validate, activate, and terminate subscriptions based on the amount and type of arguments.
- The `ISubscriptionState` interface is used by publishers to invoke delegates and keep track of pool elements the subscriptions are stored in.
- The non-alloc versions of `ISubscribables` exist so that the subscription process itself produces no allocation. For this purpose, subscriptions are pre-allocated as variables at their holder classes, and their references get pooled to the publishers' invocation pools upon subscribing.

### Pub/sub interfaces

#### Publishers

- [`IPublisherNoArgs`](IPublisherNoArgs.md) Represents a publisher that does not require any arguments when publishing events.
- [`IPublisherSingleArgGeneric<TValue>`](IPublisherSingleArgGeneric.md) Represents a publisher that can publish a single argument of generic type.
- [`IPublisherSingleArg`](IPublisherSingleArg.md) Represents a publisher that can publish events with a single argument.
- [`IPublisherMultipleArgs`](IPublisherMultipleArgs.md) Represents a publisher that can publish multiple arguments. Arguments are passed as an array of objects.

#### Subscriptions

- [`ISubscription`](ISubscription.md) Represents a subscription to a publisher.
- [`ISubscriptionHandler<TSubscribable, TInvokable>`](ISubscriptionHandler.md) Represents a handler for managing subscriptions to a specific type of subscribable object.
- [`ISubscriptionState<TInvokable>`](ISubscriptionState.md) Represents the state of a subscription.

#### Regular publisher subscription providers (subscribables)

- [`ISubscribable`](ISubscribable.md) Represents an object that can be subscribed to by other objects.
- [`ISubscribableNoArgs`](ISubscribableNoArgs.md) Represents an interface for objects that can be subscribed to without any arguments.
- [`ISubscribableSingleArgGeneric<TValue>`](ISubscribableSingleArgGeneric.md) Represents an interface for a subscribable object that supports a single argument generic delegate.
- [`ISubscribableSingleArg`](ISubscribableSingleArg.md) Represents an interface for a subscribable object with a single argument.
- [`ISubscribableMultipleArgs`](ISubscribableMultipleArgs.md) Represents an interface for objects that can be subscribed to with multiple arguments. Arguments are passed as an array of objects.

#### Non-alloc publisher subscription providers (subscribables)

- [`INonAllocSubscribable`](INonAllocSubscribable.md) Represents a subscribable object that allows non-allocating subscriptions.
- [`INonAllocSubscribableNoArgs`](INonAllocSubscribableNoArgs.md) Represents an interface for a non-allocating subscribable with no arguments.
- [`INonAllocSubscribableSingleArgGeneric<TValue>`](INonAllocSubscribableSingleArgGeneric.md) Represents a non-allocating subscribable interface with a single generic argument.
- [`INonAllocSubscribableSingleArg`](INonAllocSubscribableSingleArg.md) Represents an interface for a non-allocating subscribable with a single argument.
- [`INonAllocSubscribableMultipleArgs`](INonAllocSubscribableMultipleArgs.md) Represents an interface for a non-allocating subscribable with multiple arguments. Arguments are passed as an array of objects.
