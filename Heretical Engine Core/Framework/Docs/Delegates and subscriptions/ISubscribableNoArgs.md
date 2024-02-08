# ISubscribableNoArgs

Represents an interface for objects that can be subscribed to without any arguments. Inherits from [`ISubscribable`](ISubscribable.md). For the non-allocating version, see [`INonAllocSubscribableNoArgs`](INonAllocSubscribableNoArgs.md).

## Methods

Method | Description
--- | ---
`void Subscribe(Action @delegate)` | Subscribes to the event with the specified delegate
`void Unsubscribe(Action @delegate)` | Unsubscribes from the event with the specified delegate
`IEnumerable<Action> AllSubscriptions { get; }` | Gets all the subscriptions for this object

## Using ISubscribableNoArgs

### List all subscriptions

```csharp
ISubscribableNoArgs foo;

IEnumerable<Action> allSubscriptions = foo.AllSubscriptions;
```

### Subscribe a subscription

```csharp
ISubscribableNoArgs foo;

void Bar();

foo.Subscribe(Bar);
```

### Unsubscribe a subscription

```csharp
ISubscribableNoArgs foo;

void Bar();

foo.Unsubscribe(Bar);
```

## Creating ISubscribableNoArgs

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a subscribable with no arguments
ISubscribableNoArgs foo = DelegatesFactory.BuildPinger(loggerResolver);
```

## Implementing ISubscribableNoArgs

```csharp
private Action multicastDelegate;

#region ISubscribableNoArgs

public void Subscribe(Action @delegate)
{
	multicastDelegate += @delegate;
}

public void Unsubscribe(Action @delegate)
{
	multicastDelegate -= @delegate;
}

IEnumerable<Action> ISubscribableNoArgs.AllSubscriptions
{
	get
	{
		return multicastDelegate?
			.GetInvocationList()
			.Cast<Action>()
			?? Enumerable.Empty<Action>();
	}
}

#endregion
```
