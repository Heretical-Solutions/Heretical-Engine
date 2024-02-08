# ISubscribableSingleArg

Represents an interface for a subscribable object with a single argument. Inherits from [`ISubscribable`](ISubscribable.md). For the non-allocating version, see [`INonAllocSubscribableSingleArg`](INonAllocSubscribableSingleArg.md)

## Methods

Method | Description
--- | ---
`void Subscribe<T>(Action<T> @delegate)` | Subscribes to the event with a delegate that takes a single argument of type T
`void Subscribe(Type valueType, object @delegate)` | Subscribes to the event with a delegate that takes a single argument of the specified valueType
`void Unsubscribe<T>(Action<T> @delegate)` | Unsubscribes from the event with a delegate that takes a single argument of type T
`void Unsubscribe(Type valueType, object @delegate)` | Unsubscribes from the event with a delegate that takes a single argument of the specified valueType
`IEnumerable<Action<T>> GetAllSubscriptions<T>()` | Gets all the subscriptions for the event with delegates that take a single argument of type T
`IEnumerable<object> GetAllSubscriptions(Type valueType)` | Gets all the subscriptions for the event with delegates that take a single argument of the specified valueType

## Using ISubscribableSingleArg

### List all subscriptions of type T

```csharp
ISubscribableSingleArg foo;

IEnumerable<T> allSubscriptions = foo.GetAllSubscriptions<T>();

IEnumerable<object> allSubscriptions = foo.GetAllSubscriptions(typeof(T));
```

### Subscribe a subscription

```csharp
ISubscribableSingleArg foo;

void Bar(T argument);

foo.Subscribe<T>(Bar);

foo.Subscribe(typeof(T), Bar);
```

### Unsubscribe a subscription

```csharp
ISubscribableSingleArg foo;

void Bar(T argument);

foo.Unsubscribe<T>(Bar);

foo.Unsubscribe(typeof(T), Bar);
```

## Creating ISubscribableSingleArg

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//An object repository is needed for the broadcaster to select the concrete broadcaster for a given argument type
IReadOnlyObjectRepository repository;

//Create a subscribable with a single argument
ISubscribableSingleArg foo = DelegatesFactory.BuildBroadcasterWithRepository(
    repository,
    loggerResolver);

//Create a subscribable with a single generic argument
ISubscribableSingleArg foo = DelegatesFactory.BuildBroadcasterGeneric<T>(loggerResolver);
```

## Implementing ISubscribableSingleArg

```csharp
#region ISubscribableSingleArg
		
public void Subscribe<T>(Action<T> @delegate)
{
	var valueType = typeof(T);
	
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (ISubscribableSingleArgGeneric<T>)broadcasterObject;
	
	broadcaster.Subscribe(@delegate);
}

public void Subscribe(
	Type valueType,
	object @delegate)
{
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (ISubscribableSingleArg)broadcasterObject;
	
	broadcaster.Subscribe(valueType, @delegate);
}

public void Unsubscribe<T>(Action<T> @delegate)
{
	var valueType = typeof(T);
	
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (ISubscribableSingleArgGeneric<T>)broadcasterObject;
	
	broadcaster.Unsubscribe(@delegate);
}

public void Unsubscribe(
	Type valueType,
	object @delegate)
{
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (ISubscribableSingleArg)broadcasterObject;
	
	broadcaster.Unsubscribe(valueType, @delegate);
}

public IEnumerable<Action<T>> GetAllSubscriptions<T>()
{
	var valueType = typeof(T);

	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (ISubscribableSingleArgGeneric<T>)broadcasterObject;

	return broadcaster.AllSubscriptions;
}

public IEnumerable<object> GetAllSubscriptions(Type valueType)
{
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (ISubscribable)broadcasterObject;

	return broadcaster.AllSubscriptions;
}

#endregion
```
