# IInvokableSingleArgGeneric\<T\>

Represents an interface for invoking a method with a single argument of type `T`

## Methods

Method | Description
--- | ---
`void Invoke(T arg)` | Invokes the method with the specified argument of type `T`
`void Invoke(object arg)` | Invokes the method with the specified argument of type `object`

## Using IInvokableSingleArgGeneric\<T\>

### Invoke a delegate

```csharp
IInvokableSingleArgGeneric<T> foo;

T bar;

foo.Invoke(bar);

object bar;

foo.Invoke(bar);
```

## Creating IInvokableSingleArgGeneric\<T\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

void Foo(T value);

//Create an invokable with a single argument of type T
IInvokableSingleArgGeneric<T> bar = DelegatesFactory.BuildDelegateWrapperSingleArgGeneric<TValue>(
    Foo,
    loggerResolver);
```

## Implementing IInvokableSingleArgGeneric\<T\>

```csharp
private Action<TValue> @delegate;
        
#region IInvokableSingleArgGeneric

public void Invoke(TValue argument)
{
	@delegate?.Invoke(argument);
}

public void Invoke(object argument)
{
	@delegate?.Invoke((TValue)argument);
}

#endregion
```
