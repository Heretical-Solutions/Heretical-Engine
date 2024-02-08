# [Work in progress]

---

# Repositories

## TL;DR

- Use [`IRepository<TKey, TValue>`](IRepository.md) just like you would use `Dictionary<TKey, TValue>`. The interface is there to provide dictionary methods for different types of underlying storages, whether it be associative arrays or something completely different.
- Use [`IReadOnlyRepository<TKey, TValue>`](IReadOnlyRepository.md) just like you would use `Dictionary<TKey, TValue>`, but with limited access to methods that do not change the state of the repository, making it 'read only'.
- Use [`IObjectRepository`](IObjectRepository.md) just like `Dictionary<Type, object>` to map a type to its instance.
- Use [`IReadOnlyObjectRepository`](IReadOnlyObjectRepository.md) just like `Dictionary<Type, object>` to map a type to its instance, but with limited access to methods that do not change the state of the repository, making it 'read only'.

## Implementations

- `DictionaryRepository<TKey, TValue>` basically uses C#'s `Dictionary<TKey, TValue>` to implement `IRepository<TKey, TValue>`.
- `DictionaryObjectRepository<TKey, TValue>` basically uses C#'s `Dictionary<TKey, TValue>` to implement `IObjectRepository<TKey, TValue>`.
- `ConcurrentDictionaryRepository<TKey, TValue>` basically uses C#'s `ConcurrentDictionary<TKey, TValue>` to implement `IRepository<TKey, TValue>`.
- `ConcurrentDictionaryObjectRepository<TKey, TValue>` basically uses C#'s `ConcurrentDictionary<TKey, TValue>` to implement `IObjectRepository<TKey, TValue>`.
