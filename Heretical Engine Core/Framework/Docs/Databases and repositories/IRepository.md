# IRepository\<TKey, TValue\>

Represents a generic repository interface that provides basic CRUD operations for a collection of key-value pairs. Inherits from [`IReadOnlyRepository<TKey, TValue>`](IReadOnlyRepository.md).

## Methods

Method | Description
--- | ---
`void Add(TKey key, TValue value)` | Adds a new key-value pair to the repository
`bool TryAdd(TKey key, TValue value)` | Tries to add a new key-value pair to the repository
`void Update(TKey key, TValue value)` | Updates the value of an existing key-value pair in the repository
`bool TryUpdate(TKey key, TValue value)` | Tries to update the value of an existing key-value pair in the repository
`void AddOrUpdate(TKey key, TValue value)` | Adds a new key-value pair to the repository or updates the value of an existing pair
`void Remove(TKey key)` | Removes a key-value pair from the repository
`bool TryRemove(TKey key)` | Tries to remove a key-value pair from the repository
`void Clear()` | Removes all key-value pairs from the repository

## Using IRepository\<TKey, TValue\>

### Adding and removing values

```csharp
IRepository<TKey, TValue> foo;

TKey key;

TValue value;

//Add value
foo.Add(key, value);

//Update value
foo.Update(key, value);

//Add or update
foo.AddOrUpdate(key, value);

//Remove value
foo.Remove(key);

//Clear all values
foo.Clear();
```

## Creating IRepository\<TKey, TValue\>

```csharp
IRepository<TKey, TValue> foo = RepositoriesFactory.BuildDictionaryRepository<TKey, TValue>();
```

## Implementing IRepository\<TKey, TValue\>

```csharp
private Dictionary<TKey, TValue> database;

#region IRepository

public void Add(
	TKey key,
	TValue value)
{
	database.Add(
		key,
		value);
}

public bool TryAdd(
	TKey key,
	TValue value)
{
	return database.TryAdd(
		key,
		value);
}

public void Update(
	TKey key,
	TValue value)
{
	database[key] = value;
}

public bool TryUpdate(
	TKey key,
	TValue value)
{
	if (!Has(key))
		return false;

	Update(
		key,
		value);

	return true;
}

public void AddOrUpdate(
	TKey key,
	TValue value)
{
	if (Has(key))
		Update(
			key,
			value);
	else
		Add(
			key,
			value);
}

public void Remove(TKey key)
{
	database.Remove(key);
}

public bool TryRemove(TKey key)
{
	if (!Has(key))
		return false;

	Remove(key);

	return true;
}


public void Clear()
{
	database.Clear();
}

#endregion
```
