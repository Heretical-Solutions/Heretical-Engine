# [Work in progress]

---

# Allocations

## TL;DR

- [`AllocationCommand\<T\>`](AllocationCommand.md) is a `command` pattern class that encapsulates the descriptor struct and the delegate to to perform the allocation (creation) of (whatever)
- [`AllocationDescriptor`](AllocationDescriptor.md) is a struct describing how many instances of (whatever) to allocate (create). When used in the "additional allocations" role, it may specify how many instances to allocate additionally compared to how many are allocated right now
- `AllocationsFactory` contains basic allocation methods that can be used in the AllocationCommand delegate