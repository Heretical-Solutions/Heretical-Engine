# IInvokableMultipleArgs

Represents an interface for invoking a method with multiple arguments

## Methods

Method | Description
--- | ---
`void Invoke(object[] args)` | Invokes the method with the specified arguments

## Using IInvokableMultipleArgs

### Invoke a delegate

```csharp
IInvokableMultipleArgs foo;

object[] bar;

foo.Invoke(bar);
```

## Creating IInvokableMultipleArgs

```csharp
void Foo(object[] args);

//Create an invokable with multiple arguments
IInvokableMultipleArgs bar = DelegatesFactory.BuildDelegateWrapperMultipleArgs(Foo)
```

## Implementing IInvokableMultipleArgs

```csharp
private Action<object[]> @delegate;

#region IInvokableMultipleArgs

public void Invoke(object[] arguments)
{
	@delegate?.Invoke(arguments);
}

#endregion
```
