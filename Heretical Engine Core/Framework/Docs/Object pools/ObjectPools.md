# [Work in progress]

---

# Object pools

## TL;DR

- Use [`IPool<T>`](IPool.md) for basic object pool needs (Pop and Push)
- Use [`INonAllocPool<T>`](INonAllocPool.md) for object pools that do not allocate additional memory unless their capacity is exceeded
- Use [`IDecoratedPool<T>`](IDecoratedPool.md) and [`INonAllocDecoratedPool<T>`](INonAllocDecoratedPool.md) to perform additional changes to elements while popping from pool, pushing to pool or resizing the pool
