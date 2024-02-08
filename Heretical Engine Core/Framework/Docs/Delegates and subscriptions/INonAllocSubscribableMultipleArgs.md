# INonAllocSubscribableMultipleArgs

Represents an interface for a non-allocating subscribable with multiple arguments. Arguments are passed as an array of objects. Inherits from [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version see [`ISubscribableMultipleArgs`](ISubscribableMultipleArgs.md).

## Methods

Method | Description
--- | ---
`void Subscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription)` | Subscribes a subscription handler to the non-allocating subscribable
`void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription)` | Unsubscribes a subscription handler from the non-allocating subscribable
`IEnumerable<ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs>> AllSubscriptions { get; }` | Gets all the subscriptions of the non-allocating subscribable

## Using INonAllocSubscribableMultipleArgs

### List all subscriptions

```csharp
INonAllocSubscribableMultipleArgs foo;

var allSubscriptions = foo.AllSubscriptions;
```

### Subscribe a subscription

```csharp
INonAllocSubscribableMultipleArgs foo;

void Bar(object[] arguments);

//Create a subscription with multiple arguments
ISubscription subscription = DelegatesFactory.BuildSubscriptionMultipleArgs(
    Bar,
    loggerResolver)

foo.Subscribe(
	(ISubscriptionHandler<
        INonAllocSubscribableMultipleArgs,
        IInvokableMultipleArgs>)
		subscription);
```

### Unsubscribe a subscription

```csharp
INonAllocSubscribableMultipleArgs foo;

ISubscription subscription;

foo.Unsubscribe(
	(ISubscriptionHandler<
        INonAllocSubscribableMultipleArgs,
        IInvokableMultipleArgs>)
		subscription);
```

## Creating INonAllocSubscribableMultipleArgs

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a non alloc subscribable with multiple arguments
INonAllocSubscribableMultipleArgs foo = DelegatesFactory.BuildNonAllocBroadcasterMultipleArgs(loggerResolver);
```

## Implementing INonAllocSubscribableMultipleArgs

```csharp
#region INonAllocSubscribableMultipleArgs

public void Subscribe(
	ISubscriptionHandler<
		INonAllocSubscribableMultipleArgs,
		IInvokableMultipleArgs>
		subscription)
{
	if (!subscription.ValidateActivation(this))
		return;

	var subscriptionElement = subscriptionsPool.Pop();

	var subscriptionState = (ISubscriptionState<IInvokableMultipleArgs>)subscription;

	subscriptionElement.Value = (ISubscription)subscriptionState;

	subscription.Activate(this, subscriptionElement);
}

public void Unsubscribe(
	ISubscriptionHandler<
		INonAllocSubscribableMultipleArgs,
		IInvokableMultipleArgs>
		subscription)
{
	if (!subscription.ValidateTermination(this))
		return;

	var poolElement = ((ISubscriptionState<IInvokableMultipleArgs>)subscription).PoolElement;

	poolElement.Value = null;

	subscriptionsPool.Push(poolElement);

	subscription.Terminate();
}

IEnumerable<
	ISubscriptionHandler<
		INonAllocSubscribableMultipleArgs,
		IInvokableMultipleArgs>>
		INonAllocSubscribableMultipleArgs.AllSubscriptions
{
	get
	{
		var allSubscriptions = new ISubscriptionHandler<
			INonAllocSubscribableMultipleArgs,
			IInvokableMultipleArgs>
			[subscriptionsAsIndexable.Count];

		for (int i = 0; i < allSubscriptions.Length; i++)
			allSubscriptions[i] = (ISubscriptionHandler<
				INonAllocSubscribableMultipleArgs,
				IInvokableMultipleArgs>)
				subscriptionsAsIndexable[i].Value;

		return allSubscriptions;
	}
}

#endregion
```
