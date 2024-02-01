# [Work in progress]

---

# Object pools

## TL;DR

- Use [`IPool<T>`](IPool.md) for basic object pool needs (Pop and Push)
- Use [`INonAllocPool<T>`](INonAllocPool.md) for object pools that do not allocate additional memory unless their capacity is exceeded
- Use [`IDecoratedPool<T>`](IDecoratedPool.md) and [`INonAllocDecoratedPool<T>`](INonAllocDecoratedPool.md) to perform additional changes to elements while popping from pool, pushing to pool or resizing the pool

## What are IPoolElements for

- [`IPoolElement<T>`](IPoolElement.md) is a wrapper that contains the pool element value, the metadata, the element status, the reference to the pool it belongs and a callback

## What are allocation callbacks for

## What are PushBehaviourHandlers for

## What are DeferredCallbackQueues for
//TODO: get rid of them ffs

## Implementations

### Generic

- [`StackPool<T>`](StackPool.md) uses C#'s `Stack<T>` to store generic pool elements and `Action<StackPool<T>> resizeDelegate` to modify its size. When `Pop` is invoked, an element from stack is popped and returned. If stack is empty, `resizeDelegate` is invoked first to ensure the stack has elements to provide

### Generic non-alloc

- [`PackedArrayPool<T>`](PackedArrayPool.md) uses an array of `IPoolElement<T>` wrappers to store pool elements. This collection is not resizeable on its own and if it runs out of elements to provide it throws an exception. When `Pop` is invoked, the element at `Count` index (last non-popped one) is prepared (its metadata's index is set to its index in the array, the wrapper's push behaviour handler is set to the current array and the status is updated to POPPED) and returned, the index of currently popped elements is incremented. When `Push` is invoked, the pushed element's wrapper is swapped with the last non-popped element's wrapper in the array, its metadata is deinitialized and the index of currently popped elements is decremented

