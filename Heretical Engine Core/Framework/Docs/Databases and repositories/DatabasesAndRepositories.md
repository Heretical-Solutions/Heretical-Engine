# [Work in progress]

---

# Repositories

## TL;DR

- Use [`IRepository<TKey, TValue>`](IRepository.md) just like you would use `Dictionary<TKey, TValue>`. The interface is there to provide dictionary methods for different types of underlying storages, whether it be associative arrays or something completely different.
- Use [`IReadOnlyRepository<TKey, TValue>`](IReadOnlyRepository.md) just like you would use `Dictionary<TKey, TValue>`, but with limited access to methods that do not change the state of the repository, making it 'read only'.
- Use [`IObjectRepository`](IObjectRepository.md) just like `Dictionary<Type, object>` to map a type to its instance.
- Use [`IReadOnlyObjectRepository`](IReadOnlyObjectRepository.md) just like `Dictionary<Type, object>` to map a type to its instance, but with limited access to methods that do not change the state of the repository, making it 'read only'.

## Reasoning for IRepository or why not just use Dictionary\<TKey, TValue\>

- `Dictionary<TKey, TValue>` is a great data structure for being an associative array, but it is not the only data structure that can be used for that purpose. The methods like Add, Remove or Update are not exclusive to `Dictionary<TKey, TValue>`, and they can be implemented in a different way for different data structures while still providing the same functionality. For instance,
 	- You can use relational databases like SQL Server, MySQL, PostgreSQL, etc., to store key-value pairs in tables
 	- NoSQL databases like MongoDB, Redis, Couchbase, etc., often support key-value stores or document stores that allow you to store and retrieve data based on keys efficiently
 	- You can design and implement network APIs that provide CRUD operations for key-value pairs. These APIs can be built using various technologies such as RESTful services, gRPC, GraphQL, etc
 	- Caching systems like Redis, Memcached, etc., can be used to store key-value pairs in memory for fast access
 	- Depending on your specific requirements, you can implement custom data structures that provide associative array-like functionality. For example, you can implement hash tables, trie data structures, or other associative array implementations tailored to your needs
- You may want to use a thread-safe data structure to store key-value pairs in a multi-threaded environment. `Dictionary<TKey, TValue>` is not thread-safe, so you may need to use a different data structure that provides thread safety, like `ConcurrentDictionary<TKey, TValue>`. `IRepository` unifies the interface for different data structures, so you can use the same methods to work with different data structures without changing your code


## Implementations

- `DictionaryRepository<TKey, TValue>` basically uses C#'s `Dictionary<TKey, TValue>` to implement `IRepository<TKey, TValue>`.
- `DictionaryObjectRepository<TKey, TValue>` basically uses C#'s `Dictionary<TKey, TValue>` to implement `IObjectRepository<TKey, TValue>`.
- `ConcurrentDictionaryRepository<TKey, TValue>` basically uses C#'s `ConcurrentDictionary<TKey, TValue>` to implement `IRepository<TKey, TValue>`.
- `ConcurrentDictionaryObjectRepository<TKey, TValue>` basically uses C#'s `ConcurrentDictionary<TKey, TValue>` to implement `IObjectRepository<TKey, TValue>`.

## TODO

- IAsyncRepository\<TKey, TValue\>
- IAsyncReadOnlyRepository\<TKey, TValue\>
- IAsyncObjectRepository
- IAsyncReadOnlyObjectRepository