# IInvokableNoArgs

Represents an interface for invoking a method with no arguments

## Methods

Method | Description
--- | ---
`void Invoke()` | Invokes the method with no arguments

## Using IInvokableNoArgs

### Invoke a delegate

```csharp
IInvokableNoArgs foo;

foo.Invoke();
```

## Creating IInvokableNoArgs

```csharp
void Foo();

//Create an invokable with no arguments
IInvokableNoArgs bar = DelegatesFactory.BuildDelegateWrapperNoArgs(Foo);
```

## Implementing IInvokableNoArgs

```csharp
private Action @delegate;

#region IInvokableNoArgs

public void Invoke()
{
	@delegate?.Invoke();
}

#endregion
```
