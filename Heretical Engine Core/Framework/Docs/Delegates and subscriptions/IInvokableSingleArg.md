# IInvokableSingleArg

Represents an interface for invoking a method with a single argument

## Methods

Method | Description
--- | ---
`void Invoke<T>(T value)` | Invokes the method with the specified argument
`void Invoke(Type valueType, object value)` | Invokes the method with the specified argument

## Using IInvokableSingleArg

### Invoke a delegate

```csharp
IInvokableSingleArg foo;

T bar;

foo.Invoke<T>(bar);

foo.Invoke(typeof(T), bar);
```

## Creating IInvokableSingleArg

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

void Foo(T value);

//Create an invokable with a single argument
IInvokableSingleArg bar = DelegatesFactory.BuildDelegateWrapperSingleArgGeneric<TValue>(
    Foo,
    loggerResolver);

//Create an invokable with a single argument
IInvokableSingleArg bar = DelegatesFactory.BuildDelegateWrapperSingleArg<TValue>(
    Foo,
    loggerResolver);
```

## Implementing IInvokableSingleArg

```csharp
#region IInvokableSingleArg
        
public void Invoke<TArgument>(TArgument value)
{
	switch (value)
	{
		case TValue tValue:

			@delegate.Invoke(tValue);

			break;

		default:

			throw new Exception(
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\"");
	}
}

public void Invoke(Type valueType, object value)
{
	switch (value)
	{
		case TValue tValue:

			@delegate.Invoke(tValue);

			break;

		default:

			throw new Exception(
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\"");
	}
}

#endregion
```
