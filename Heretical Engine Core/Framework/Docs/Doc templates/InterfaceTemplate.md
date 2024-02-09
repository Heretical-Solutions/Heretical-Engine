# [Work in progress]

---

# IGenericInterface\<T\>

Lorem ipsum IGenericInterface\<T\>. Inherits from [`IParentInterface`](IParentInterface.md). Contains methods that are dolor sit a met. For interfaces that are not dolor sit a met, see [`INotGenericInterface`](INotGenericInterface.md), [`IAnotherNotGenericInterface`](IAnotherNotGenericInterface.md), and [`IYetAnotherInterface<T>`](IYetAnotherInterface.md). For the non-allocating version, see [`INonAllocGenericInterface\<T\>`](INonAllocGenericInterface.md)

## Methods

Method | Description
--- | ---
`T MethodA()` | Lorem ipsum
`void MethodB(T argument)` | Lorem ipsum
`bool PropertyC { get; }` | Lorem ipsum

## Using IGenericInterface\<T\>

### Case A

```csharp
IGenericInterface<T> foo;

//lorem ipsum
foo.bar();

//lorem ipsum

//lorem ipsum
foo.bar();
```

### Case B

```csharp
IGenericInterface<T> foo;

//lorem ipsum
foo.bar();

//lorem ipsum

//lorem ipsum
foo.bar();
```

## Creating IGenericInterface\<T\>

```csharp
//lorem ipsum
IGenericInterface<T> foo = bar();
```

## Implementing IGenericInterface\<T\>

```csharp
#region IGenericInterface<T>

//lorem ipsum
public T MethodA() {}

//lorem ipsum
public void MethodB(T argument) {}

//lorem ipsum
public bool PropertyC { get; }

#endregion
```
