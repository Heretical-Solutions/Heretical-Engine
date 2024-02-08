# ISubscribable

Represents an object that can be subscribed to by other objects. Contains methods that are not dependent on the type or the amount of arguments the subscribers can pass to the publisher. For interfaces that are dependent on the amount of arguments, see [`ISubscribableNoArgs`](ISubscribableNoArgs.md), [`ISubscribableSingleArg`](ISubscribableSingleArg.md), [`ISubscribableSingleArgGeneric<T>`](ISubscribableSingleArgGeneric.md), and [`ISubscribableMultipleArgs`](ISubscribableMultipleArgs.md). For the non-allocating version, see [`INonAllocSubscribable`](INonAllocSubscribable.md).

## Methods

Method | Description
--- | ---
`IEnumerable<object> AllSubscriptions { get; }` | Gets all the current subscriptions to this object
`void UnsubscribeAll()` | Unsubscribes all objects from this object

## Using ISubscribable

### List all subscriptions

```csharp
ISubscribable foo;

IEnumerable<object> allSubscriptions = foo.AllSubscriptions;
```

### Cancel all subscriptions

```csharp
ISubscribable foo;

foo.UnsubscribeAll();
```

## Creating ISubscribable

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a subscribable with no arguments
ISubscribable foo = DelegatesFactory.BuildPinger(loggerResolver);

//Create a subscribable with a single generic argument
ISubscribable foo = DelegatesFactory.BuildBroadcasterGeneric<T>(loggerResolver);

//An object repository is needed for the broadcaster to select the concrete broadcaster for a given argument type
IReadOnlyObjectRepository repository;

//Create a subscribable with a single argument
ISubscribable foo = DelegatesFactory.BuildBroadcasterWithRepository(
    repository,
    loggerResolver);

//Create a subscribable with multiple arguments
ISubscribable foo = BuildBroadcasterMultipleArgs(loggerResolver);
```

## Implementing ISubscribable

```csharp
private Action multicastDelegate;

#region ISubscribable

IEnumerable<object> ISubscribable.AllSubscriptions
{
    get
    {
        return multicastDelegate?
			.GetInvocationList()
			.Cast<object>()
			?? Enumerable.Empty<object>();
    }
}

public void UnsubscribeAll()
{
    multicastDelegate = null;
}

#endregion
```
