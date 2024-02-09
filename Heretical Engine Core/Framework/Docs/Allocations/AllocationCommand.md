# AllocationCommand\<T\>

A `command` pattern class that encapsulates the descriptor struct and the delegate to perform the allocation (creation) of (whatever)

## Properties

Property | Description
--- | ---
`AllocationCommandDescriptor Descriptor` | Gets or sets the allocation command descriptor
`Func<T> AllocationDelegate` | Gets or sets the delegate used for the allocation

## Using AllocationCommand\<T\>

### Spawn one instance of TInterfaceType on the beginning, spawn twice as much every time after

```csharp
//The allocation delegate used in the command is creating a new instance of TConcreteType via Activator.CreateInstance method, casts it to TInterfaceType and returns an instance of TInterfaceType
Func<TInterfaceType> valueAllocationDelegate = AllocationsFactory
	.ActivatorAllocationDelegate<TInterfaceType, TConcreteType>;

//The command tells its consumer to spawn one instance of TInterfaceType with the delegate described above every time it's used
var initialAllocationCommand = new AllocationCommand<TInterfaceType>
{
    Descriptor = new AllocationCommandDescriptor
    {
        Rule = EAllocationAmountRule.ADD_ONE
    },
    AllocationDelegate = valueAllocationDelegate
};

//The command tells its consumer to spawn twice the amount of TInterfaceTypes it already has with the delegate described above every time it's used
var additionalAllocationCommand = new AllocationCommand<TInterfaceType>
{
    Descriptor = new AllocationCommandDescriptor
    {
        Rule = EAllocationAmountRule.DOUBLE_AMOUNT
    },
    AllocationDelegate = valueAllocationDelegate
};
```
