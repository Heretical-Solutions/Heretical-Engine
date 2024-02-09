# IPublisherSingleArgGeneric\<T\>

Represents a publisher that can publish a single argument of generic type

## Methods

Method | Description
--- | ---
`void Publish(T value)` | Publishes the specified value

## Using IPublisherSingleArgGeneric\<T\>

### Publish an event

```csharp
IPublisherSingleArgGeneric<T> foo;

T bar;

foo.Publish(bar);
```

## Creating IPublisherSingleArgGeneric\<T\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a publisher with no arguments
IPublisherSingleArgGeneric<T> foo = DelegatesFactory.BuildBroadcasterGeneric<T>(loggerResolver);
```

## Implementing IPublisherSingleArgGeneric\<T\>

```csharp
private Action<T> multicastDelegate;

#region IPublisherSingleArgGeneric

public void Publish(T value)
{
	//If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
	//should NOT be changed during the invokation
	//That's why we'll copy the multicast delegate to a local variable and invoke it from there
	//multicastDelegate?.Invoke(value);

	var multicastDelegateCopy = multicastDelegate;

	multicastDelegateCopy?.Invoke(value);

	multicastDelegateCopy = null;
}

#endregion
```
