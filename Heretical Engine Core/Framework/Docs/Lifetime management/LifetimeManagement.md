# [Work in progress]

---

# Lifetime management

## TL;DR

- [`ILifetimeable`](ILifetimeable.md) provides its inheritors with four distinct lifetime stages: SetUp (self-initialization, no dependencies yet), Initialize (full initialization, receives dependencies from other sources), Cleanup (deinitialization, removes dependencies but still can be initialized again) and TearDown (self-deinitialization, disposal)
- `ILifetimeable` provides properties to tell other classes (including other lifetimeables) of lifetimeable's current state and callbacks to notify them of lifetimeable's state change
- `LifetimeSynchronizer` can help lifetimeables to synchronize their lifetime with their parent lifetimeable's lifetime. For instance, that allows views and viewmodels to be initialized once their parent view/viewmodel is initialized and clean up once they're cleaned up, creating a hierarchical structure
- Use [`ISetUppable`](ISetUppable.md) for classes that should have a `SetUp` method (which performs self-initalization and sets IsSetUp
to true)
- Use [`IInitializable`](IInitializable.md) for classes that should have an `Initialize` method (which performs initalization, sets IsInitialized to true and triggers the OnInitalized callback)
- Use [`ICleanUppable`](ICleanUppable.md) for classes that should have a `Cleanup` method (which performs deinitalization, sets IsInitialized to false and triggers the OnCleanedUp callback)
- Use [`ITearDownable`](ITearDownable.md) for classes that should have a `TearDown` method (which performs disposal, sets IsSetUp to false and triggers the OnTornDown callback)
