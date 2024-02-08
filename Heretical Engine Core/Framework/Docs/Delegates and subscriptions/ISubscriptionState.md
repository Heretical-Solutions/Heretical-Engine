# ISubscriptionState\<TInvokable\>

Represents the state of a subscription. `TInvokable` should be inherited from `IInvokable` interface family

## Methods

Method | Description
--- | ---
`TInvokable Invokable { get; }` | Gets the invokable object associated with the subscription state
`IPoolElement<ISubscription> PoolElement { get; }` | Gets the pool element associated with the subscription state

## Using ISubscriptionState\<TInvokable\>

### Invoke the invokable

```csharp
ISubscription subscription;

var subscriptionState = (ISubscriptionState<IInvokableNoArgs>)subscription;

subscriptionState.Invokable.Invoke();
```

### Access the pool element

```csharp
ISubscription subscription;

var poolElement = ((ISubscriptionState<IInvokableNoArgs>)subscription).PoolElement;
```

## Creating ISubscriptionState\<TInvokable\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

void Foo();

//Create a subscription with no arguments
ISubscriptionState<IInvokableNoArgs>
	bar = DelegatesFactory.BuildSubscriptionNoArgs(
		Foo,
		loggerResolver);

void Foo(T value);

//Create a subscription with a single generic argument
ISubscriptionState<IInvokableSingleArgGeneric<T>>
	bar = DelegatesFactory.BuildSubscriptionSingleArgGeneric<T>(
		Foo,
		loggerResolver);

void Foo(object[] arguments);

//Create a subscription with multiple arguments
ISubscriptionState<IInvokableMultipleArgs>
	bar = DelegatesFactory.BuildSubscriptionMultipleArgs(
    	Foo,
    	loggerResolver)

```

## Implementing ISubscriptionState\<TInvokable\>

```csharp
private IInvokableNoArgs invokable;

private IPoolElement<ISubscription> poolElement;

#region ISubscriptionState

IInvokableNoArgs ISubscriptionState<IInvokableNoArgs>.Invokable =>
	(IInvokableNoArgs)invokable;

IPoolElement<ISubscription> ISubscriptionState<IInvokableNoArgs>.PoolElement => poolElement;

#endregion
```
