# IPublisherSingleArg

Represents a publisher that can publish events with a single argument

## Methods

Method | Description
--- | ---
`void Publish<T>(T value)` | Publishes an event with a single argument of type `T`
`void Publish(Type valueType, object value)` | Publishes an event with a single argument of the specified type

## Using IPublisherSingleArg

### Publish an event

```csharp
IPublisherSingleArg foo;

T bar;

foo.Publish<T>(bar);

foo.Publish(typeof(T), bar);
```

## Creating IPublisherSingleArg

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//An object repository is needed for the broadcaster to select the concrete broadcaster for a given argument type
IReadOnlyObjectRepository repository;

//Create a publisher with a single argument
IPublisherSingleArg foo = DelegatesFactory.BuildBroadcasterWithRepository(
    repository,
    loggerResolver);

//Create a publisher with a single generic argument
IPublisherSingleArg foo = DelegatesFactory.BuildBroadcasterGeneric<T>(loggerResolver);
```

## Implementing IPublisherSingleArg

```csharp
#region IPublisherSingleArg

public void Publish<T>(T value)
{
	var valueType = typeof(T);
	
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (IPublisherSingleArgGeneric<T>)broadcasterObject;
	
	broadcaster.Publish(value);
}

public void Publish(
	Type valueType,
	object value)
{
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (IPublisherSingleArg)broadcasterObject;
	
	broadcaster.Publish(valueType, value);
}

#endregion
```
