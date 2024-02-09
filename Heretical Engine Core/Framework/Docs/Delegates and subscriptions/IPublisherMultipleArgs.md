# IPublisherMultipleArgs

Represents a publisher that can publish multiple arguments. Arguments are passed as an array of objects

## Methods

Method | Description
--- | ---
`void Publish(object[] values)` | Publishes the specified values

## Using IPublisherMultipleArgs

### Publish an event

```csharp
IPublisherMultipleArgs foo;

object[] bar;

foo.Publish(bar);
```

## Creating IPublisherMultipleArgs

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a publisher with multiple arguments
IPublisherMultipleArgs foo = BuildBroadcasterMultipleArgs(loggerResolver);
```

## Implementing IPublisherMultipleArgs

```csharp
private BroadcasterGeneric<object[]> innerBroadcaster;

#region IPublisherMultipleArgs

public void Publish(object[] values)
{
	innerBroadcaster.Publish(values);
}

#endregion
```
