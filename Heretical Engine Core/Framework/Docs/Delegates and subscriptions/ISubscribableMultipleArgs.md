# ISubscribableMultipleArgs

Represents an interface for objects that can be subscribed to with multiple arguments. Arguments are passed as an array of objects. Inherits from [`ISubscribable`](ISubscribable.md). For the non-allocating version, see [`INonAllocSubscribableMultipleArgs`](INonAllocSubscribableMultipleArgs.md)

## Methods

Method | Description
--- | ---
`void Subscribe(Action<object[]> @delegate)` | Subscribes to the object with a delegate that takes an array of objects as arguments
`void Unsubscribe(Action<object[]> @delegate)` | Unsubscribes from the object with a delegate that takes an array of objects as arguments
`IEnumerable<Action<object[]>> AllSubscriptions { get; }` | Gets all the subscriptions to the object

## Using ISubscribableMultipleArgs

### List all subscriptions

```csharp
ISubscribableMultipleArgs foo;

IEnumerable<Action<object[]>> allSubscriptions = foo.AllSubscriptions;
```

### Subscribe a subscription

```csharp
ISubscribableMultipleArgs foo;

void Bar(object[] arguments);

foo.Subscribe(Bar);
```

### Unsubscribe a subscription

```csharp
ISubscribableMultipleArgs foo;

void Bar(object[] arguments);

foo.Unsubscribe(Bar);
```

## Creating ISubscribableMultipleArgs

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a subscribable with multiple arguments
ISubscribableMultipleArgs foo = BuildBroadcasterMultipleArgs(loggerResolver);
```

## Implementing ISubscribableMultipleArgs

```csharp
private BroadcasterGeneric<object[]> innerBroadcaster;

#region ISubscribableMultipleArgs
        
public void Subscribe(Action<object[]> @delegate)
{
	innerBroadcaster.Subscribe(@delegate);
}

public void Unsubscribe(Action<object[]> @delegate)
{
	innerBroadcaster.Unsubscribe(@delegate);
}

IEnumerable<Action<object[]>> ISubscribableMultipleArgs.AllSubscriptions
{
	get
	{
		return innerBroadcaster.GetAllSubscriptions<object[]>();
	}
}

#endregion
```
