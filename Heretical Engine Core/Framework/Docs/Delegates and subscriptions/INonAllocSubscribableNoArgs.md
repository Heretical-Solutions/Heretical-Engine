# INonAllocSubscribableNoArgs

Represents an interface for a non-allocating subscribable with no arguments. Inherits from [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version, see [`ISubscribableNoArgs`](ISubscribableNoArgs.md)

## Methods

Method | Description
--- | ---
`void Subscribe(ISubscription subscription)` | Subscribes to the event with the given subscription
`void Unsubscribe(ISubscription subscription)` | Unsubscribes from the event with the given subscription

## Using INonAllocSubscribableNoArgs

### Subscribe a subscription

```csharp
INonAllocSubscribableNoArgs foo;

void Bar(object[] arguments);

//Create a subscription with multiple arguments
ISubscription subscription = DelegatesFactory.BuildSubscriptionMultipleArgs(
    Bar,
    loggerResolver)

foo.Subscribe(subscription);
```

### Unsubscribe a subscription

```csharp
INonAllocSubscribableNoArgs foo;

ISubscription subscription;

foo.Unsubscribe(subscription);
```

## Creating INonAllocSubscribableNoArgs

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a subscribable with no arguments
INonAllocSubscribableNoArgs foo = DelegatesFactory.BuildNonAllocPinger(loggerResolver);
```

## Implementing INonAllocSubscribableNoArgs

```csharp
#region INonAllocSubscribableNoArgs

public void Subscribe(ISubscription subscription)
{
	var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

	if (!subscriptionHandler.ValidateActivation(this))
		return;

	var subscriptionElement = subscriptionsPool.Pop();

	subscriptionElement.Value = subscription;

	subscriptionHandler.Activate(this, subscriptionElement);
}

public void Unsubscribe(ISubscription subscription)
{
	var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

	if (!subscriptionHandler.ValidateTermination(this))
		return;

	var poolElement = ((ISubscriptionState<IInvokableNoArgs>)subscription).PoolElement;

	poolElement.Value = null;

	subscriptionsPool.Push(poolElement);

	subscriptionHandler.Terminate();
}

public void Unsubscribe(IPoolElement<ISubscription> subscription)
{
	subscription.Value = null;

	subscriptionsPool.Push(subscription);
}

#endregion
```
