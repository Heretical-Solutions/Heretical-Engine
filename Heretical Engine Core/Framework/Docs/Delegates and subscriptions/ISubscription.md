# ISubscription

Represents a subscription to a publisher. Contains methods that are not dependent on the type or the amount of arguments the subscribers can pass to the publisher

## Methods

Method | Description
--- | ---
`bool Active { get; }` | Gets a value indicating whether the subscription is active

## Creating ISubscription

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

void Foo();

//Create a subscription with no arguments
ISubscription bar = DelegatesFactory.BuildSubscriptionNoArgs(
	Foo,
	loggerResolver);

void Foo(T value);

//Create a subscription with a single generic argument
ISubscription bar = DelegatesFactory.BuildSubscriptionSingleArgGeneric<T>(
	Foo,
	loggerResolver);

void Foo(object[] arguments);

//Create a subscription with multiple arguments
ISubscription bar = DelegatesFactory.BuildSubscriptionMultipleArgs(
    Foo,
    loggerResolver)

```

## Implementing ISubscription

```csharp
#region ISubscription

public bool Active { get; private set; }

#endregion
```
