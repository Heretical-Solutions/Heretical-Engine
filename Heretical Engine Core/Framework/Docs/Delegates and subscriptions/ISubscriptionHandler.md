# ISubscriptionHandler\<TSubscribable, TInvokable\>

Represents a handler for managing subscriptions to a specific type of subscribable object. `TSubscribable` should be inherited from either [`ISubscribable`](ISubscribable.md) or [`INonAllocSubscribable`](INonAllocSubscribable.md) while `TInvokable` should be inherited from `IInvokable` interface family

## Methods

Method | Description
--- | ---
`bool ValidateActivation(TSubscribable publisher)` | Validates if the subscription can be activated for the specified publisher
`void Activate(TSubscribable publisher, IPoolElement<ISubscription> poolElement)` | Activates the subscription for the specified publisher
`bool ValidateTermination(TSubscribable publisher)` | Validates if the subscription can be terminated for the specified publisher
`void Terminate()` | Terminates the subscription

## Using ISubscriptionHandler\<TSubscribable, TInvokable\>

### Activate a subscription

```csharp
ISubscription subscription;

var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

if (!subscriptionHandler.ValidateActivation(this))
	return;

var subscriptionElement = subscriptionsPool.Pop();

subscriptionElement.Value = subscription;

subscriptionHandler.Activate(this, subscriptionElement);
```

### Terminate a subscription

```csharp
ISubscription subscription;

var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

if (!subscriptionHandler.ValidateTermination(this))
	return;

var poolElement = ((ISubscriptionState<IInvokableNoArgs>)subscription).PoolElement;

poolElement.Value = null;

subscriptionsPool.Push(poolElement);

subscriptionHandler.Terminate();
```

## Creating ISubscriptionHandler\<TSubscribable, TInvokable\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

void Foo();

//Create a subscription with no arguments
ISubscriptionHandler<
	INonAllocSubscribableNoArgs,
	IInvokableNoArgs>
	bar = DelegatesFactory.BuildSubscriptionNoArgs(
		Foo,
		loggerResolver);

void Foo(T value);

//Create a subscription with a single generic argument
ISubscriptionHandler<
	INonAllocSubscribableSingleArgGeneric<T>, IInvokableSingleArgGeneric<T>>
	bar = DelegatesFactory.BuildSubscriptionSingleArgGeneric<T>(
		Foo,
		loggerResolver);

void Foo(object[] arguments);

//Create a subscription with multiple arguments
ISubscriptionHandler<
	INonAllocSubscribableMultipleArgs,
	IInvokableMultipleArgs>
	bar = DelegatesFactory.BuildSubscriptionMultipleArgs(
    	Foo,
    	loggerResolver)

```

## Implementing ISubscriptionHandler\<TSubscribable, TInvokable\>

```csharp
#region ISubscriptionHandler

public bool ValidateActivation(
	INonAllocSubscribableSingleArg publisher)
{
	if (Active)
		throw new Exception(
			"ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");

	if (this.publisher != null)
		throw new Exception(
			"SUBSCRIPTION ALREADY HAS A PUBLISHER");

	if (poolElement != null)
		throw new Exception(
			"SUBSCRIPTION ALREADY HAS A POOL ELEMENT");

	if (invokable == null)
		throw new Exception(
			"INVALID DELEGATE");

	return true;
}

public void Activate(
	INonAllocSubscribableSingleArg publisher,
	IPoolElement<ISubscription> poolElement)
{
	this.poolElement = poolElement;

	this.publisher = publisher;

	Active = true;
}

public bool ValidateTermination(
	INonAllocSubscribableSingleArg publisher)
{
	if (!Active)
		throw new Exception(
			"ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");

	if (this.publisher != publisher)
		throw new Exception(
			"INVALID PUBLISHER");

	if (poolElement == null)
		throw new Exception(
			"INVALID POOL ELEMENT");

	return true;
}

public void Terminate()
{
	poolElement = null;

	publisher = null;

	Active = false;
}

#endregion
```
