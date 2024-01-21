# [Work in progress]

---

# IPool\<T\>

A generic interface that provides user with an object pool functionality

## Methods

Method | Description
--- | ---
`T Pop()` | Retrieves an object from the pool
`void Push(T instance)` | Returns an object to the pool
`bool HasFreeSpace { get; }` | Gets a value indicating whether the pool has free space

## Using IPool\<T\>

```csharp
IPool<TElement> pool;

//pop element from object pool
var element = pool.Pop();

//use element

//push it back
pool.Push(element);
```

## Creating IPool\<T\>

```csharp
//This delegate describes how a new pool element should be created - in this case, via Activator.CreateInstance
//For all predefined allocation delegates visit AllocationsFactory class
Func<TElement> valueAllocationDelegate = AllocationsFactory.ActivatorAllocationDelegate<TElement>;

//This command defines how many pool elements shall be created for the pool initially
var initialAllocationCommand = new AllocationCommand<TElement>
{
    Descriptor = new AllocationCommandDescriptor
    {
        Rule = EAllocationAmountRule.ADD_ONE
    },
    AllocationDelegate = valueAllocationDelegate
};

//This command defines how many pool elements shall be created additionally and added to pool once its capacity is exceeded
var additionalAllocationCommand = new AllocationCommand<TElement>
{
    Descriptor = new AllocationCommandDescriptor
    {
        Rule = EAllocationAmountRule.DOUBLE_AMOUNT
    },
    AllocationDelegate = valueAllocationDelegate
};

//StackPool implements IPool<T> and uses a simple Stack<T> under the hood
//To see all options
IPool<TElement> objectPool = PoolsFactory.BuildStackPool<TElement>(
    initialAllocationCommand,
    additionalAllocationCommand,
    null);
```
