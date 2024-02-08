# INonAllocSubscribableSingleArgGeneric\<T\>

Represents a non-allocating subscribable interface with a single generic argument. Inherits from [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version, see [`ISubscribableSingleArgGeneric<T>`](ISubscribableSingleArgGeneric.md)

## Methods

Method | Description
--- | ---
`void Subscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<T>, IInvokableSingleArgGeneric<T>> subscription)` | Subscribes a subscription handler to this subscribable
`void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<T>, IInvokableSingleArgGeneric<T>> subscription)` | Unsubscribes a subscription handler from this subscribable
`IEnumerable<ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<T>, IInvokableSingleArgGeneric<T>>> AllSubscriptions { get; }` | Gets all the subscriptions currently registered with this subscribable

## Using INonAllocSubscribableSingleArgGeneric\<T\>

### List all subscriptions

```csharp
INonAllocSubscribableSingleArgGeneric<T> foo;

var allSubscriptions = foo.AllSubscriptions;
```

### Subscribe a subscription

```csharp
INonAllocSubscribableSingleArgGeneric<T> foo;

void Bar(T argument);

//Create a subscription with a single generic argument
ISubscription subscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<T>(
	Bar,
	loggerResolver);

foo.Subscribe(
	(ISubscriptionHandler<
		INonAllocSubscribableSingleArgGeneric<TDelta>,
		IInvokableSingleArgGeneric<TDelta>>)
		subscription);
```

### Unsubscribe a subscription

```csharp
INonAllocSubscribableSingleArgGeneric<T> foo;

ISubscription subscription;

foo.Unsubscribe(
	(ISubscriptionHandler<
		INonAllocSubscribableSingleArgGeneric<TDelta>,
		IInvokableSingleArgGeneric<TDelta>>)
		subscription);
```

## Creating INonAllocSubscribableSingleArgGeneric\<T\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a non alloc subscribable with a single generic argument
INonAllocSubscribableSingleArgGeneric<T> foo = DelegatesFactory.BuildNonAllocBroadcasterGeneric<T>(loggerResolver);
```

## Implementing INonAllocSubscribableSingleArgGeneric\<T\>

```csharp
#region INonAllocSubscribableSingleArgGeneric

public void Subscribe(
	ISubscriptionHandler<
		INonAllocSubscribableSingleArgGeneric<T>,
		IInvokableSingleArgGeneric<T>>
		subscription)
{
	if (!subscription.ValidateActivation(this))
		return;

	var subscriptionElement = subscriptionsPool.Pop();

	var subscriptionState = (ISubscriptionState<IInvokableSingleArgGeneric<T>>)subscription;

	subscriptionElement.Value = (ISubscription)subscriptionState;

	subscription.Activate(this, subscriptionElement);
}

public void Unsubscribe(
	ISubscriptionHandler<
		INonAllocSubscribableSingleArgGeneric<T>,
		IInvokableSingleArgGeneric<T>>
		subscription)
{
	if (!subscription.ValidateTermination(this))
		return;

	var poolElement = ((ISubscriptionState<IInvokableSingleArgGeneric<T>>)subscription).PoolElement;

	poolElement.Value = null;

	subscriptionsPool.Push(poolElement);

	subscription.Terminate();
}

IEnumerable<
	ISubscriptionHandler<
		INonAllocSubscribableSingleArgGeneric<T>,
		IInvokableSingleArgGeneric<T>>>
		INonAllocSubscribableSingleArgGeneric<T>.AllSubscriptions
{
	get
	{
		var allSubscriptions = new ISubscriptionHandler<
			INonAllocSubscribableSingleArgGeneric<T>,
			IInvokableSingleArgGeneric<T>>
			[subscriptionsAsIndexable.Count];

		for (int i = 0; i < allSubscriptions.Length; i++)
			allSubscriptions[i] = (ISubscriptionHandler<
				INonAllocSubscribableSingleArgGeneric<T>,
				IInvokableSingleArgGeneric<T>>)
				subscriptionsAsIndexable[i].Value;

		return allSubscriptions;
	}
}

#endregion
```
