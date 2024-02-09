# IPublisherNoArgs

Represents a publisher that does not require any arguments when publishing events.

## Methods

Method | Description
--- | ---
`void Publish()` | Publishes an event without any arguments

## Using IPublisherNoArgs

### Publish an event

```csharp
IPublisherNoArgs foo;

foo.Publish();
```

## Creating IPublisherNoArgs

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a publisher with no arguments
IPublisherNoArgs foo = DelegatesFactory.BuildPinger(loggerResolver);
```

## Implementing IPublisherNoArgs

```csharp
private Action multicastDelegate;

#region IPublisherNoArgs

public void Publish()
{
	//If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
	//should NOT be changed during the invokation
	//That's why we'll copy the multicast delegate to a local variable and invoke it from there
	//multicastDelegate?.Invoke();

	var multicastDelegateCopy = multicastDelegate;

	multicastDelegateCopy?.Invoke();

	multicastDelegateCopy = null;
}

#endregion
```
