# AllocationDescriptor

A struct describing how many instances of (whatever) to allocate (create). When used in "additional allocations" role it may tell how many instances to allocate additionally compared to how many are allocated right now

## Fields

Field | Description
--- | ---
`EAllocationAmountRule Rule` | The allocation amount rule for the allocation command
`int Amount` | The amount to allocate based on the allocation command

## Using AllocationDescriptor

### Spawn one instance of IMessage on the beginning, spawn twice as much every time after

```csharp
//The descriptor tells its consumer to spawn one instance of (whatever)
var descriptor = new AllocationCommandDescriptor
{
    Rule = EAllocationAmountRule.ADD_ONE
};

//The descriptor tells its consumer to spawn twice the amount of (whatever) it already has
var descriptor = new AllocationCommandDescriptor
{
    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
};
```
