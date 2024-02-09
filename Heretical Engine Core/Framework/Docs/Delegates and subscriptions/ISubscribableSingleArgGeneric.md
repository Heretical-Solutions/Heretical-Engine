# ISubscribableSingleArgGeneric\<T\>

Represents an interface for a subscribable object that supports a single argument generic delegate. Inherits from [`ISubscribable`](ISubscribable.md). For the non-allocating version, see [`INonAllocSubscribableSingleArgGeneric<T>`](INonAllocSubscribableSingleArgGeneric.md).

## Methods

Method | Description
--- | ---
`void Subscribe(Action<T> @delegate)` | Subscribes to the delegate with the specified action
`void Unsubscribe(Action<T> @delegate)` | Unsubscribes from the delegate with the specified action
`IEnumerable<Action<T>> AllSubscriptions { get; }` | Gets all the subscriptions for the delegate

## Using ISubscribableSingleArgGeneric\<T\>

### List all subscriptions

```csharp
ISubscribableSingleArgGeneric<T> foo;

IEnumerable<Action<T>> allSubscriptions = foo.AllSubscriptions;
```

### Subscribe a subscription

```csharp
ISubscribableSingleArgGeneric<T> foo;

void Bar(T argument);

foo.Subscribe(Bar);
```

### Unsubscribe a subscription

```csharp
ISubscribableSingleArgGeneric<T> foo;

void Bar(T argument);

foo.Unsubscribe(Bar);
```

## Creating ISubscribableSingleArgGeneric\<T\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a subscribable with no arguments
ISubscribableSingleArgGeneric<T> foo = DelegatesFactory.BuildBroadcasterGeneric<T>(loggerResolver);
```

## Implementing ISubscribableSingleArgGeneric\<T\>

```csharp
private Action<T> multicastDelegate;

#region ISubscribableSingleArgGeneric

public void Subscribe(Action<T> @delegate)
{
	multicastDelegate += @delegate;
}

public void Unsubscribe(Action<T> @delegate)
{
	multicastDelegate -= @delegate;
}

IEnumerable<Action<T>> ISubscribableSingleArgGeneric<T>.AllSubscriptions
{
	get
	{
		return multicastDelegate?
			.GetInvocationList()
			.Cast<Action<T>>()
			?? Enumerable.Empty<Action<T>>();
	}
}

#endregion
```
