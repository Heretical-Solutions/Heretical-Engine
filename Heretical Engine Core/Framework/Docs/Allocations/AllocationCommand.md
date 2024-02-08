# AllocationCommand\<T\>

A `command` pattern class that encapsulates the descriptor struct and the delegate to perform the allocation (creation) of (whatever)

## Properties

Property | Description
--- | ---
`AllocationCommandDescriptor Descriptor` | Gets or sets the allocation command descriptor
`Func<T> AllocationDelegate` | Gets or sets the delegate used for the allocation

## Using AllocationCommand\<T\>

### Spawn one instance of IMessage on the beginning, spawn twice as much every time after

```csharp
//The allocation delegate used in the command is creating a new instance of TMessage via Activator.CreateInstance method, casts it to IMessage and returns an instance of IMessage
Func<IMessage> valueAllocationDelegate = AllocationsFactory
	.ActivatorAllocationDelegate<IMessage, TMessage>;

//The command tells its consumer to spawn one instance of IMessage with the delegate described above every time it's used
var initialAllocationCommand = new AllocationCommand<IMessage>
{
    Descriptor = new AllocationCommandDescriptor
    {
        Rule = EAllocationAmountRule.ADD_ONE
    },
    AllocationDelegate = valueAllocationDelegate
};

//The command tells its consumer to spawn twice the amount of IMessages it already has with the delegate described above every time it's used
var additionalAllocationCommand = new AllocationCommand<IMessage>
{
    Descriptor = new AllocationCommandDescriptor
    {
        Rule = EAllocationAmountRule.DOUBLE_AMOUNT
    },
    AllocationDelegate = valueAllocationDelegate
};
```