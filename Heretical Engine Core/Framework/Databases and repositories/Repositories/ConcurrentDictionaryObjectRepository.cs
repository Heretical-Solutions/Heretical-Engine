using System.Collections.Concurrent;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Repositories
{
	public class ConcurrentDictionaryObjectRepository
		: IObjectRepository,
		  ICloneableObjectRepository,
		  ICleanUppable,
		  IDisposable
	{
		private readonly ConcurrentDictionary<Type, object> database;

		public ConcurrentDictionaryObjectRepository(
			ConcurrentDictionary<Type, object> database)
		{
			this.database = database;
		}

		#region IObjectRepository

		#region IReadOnlyObjectRepository

		public bool Has<TValue>()
		{
			return database.ContainsKey(typeof(TValue));
		}

		public bool Has(Type valueType)
		{
			return database.ContainsKey(valueType);
		}


		public TValue Get<TValue>()
		{
			return (TValue)database[typeof(TValue)];
		}

		public object Get(Type valueType)
		{
			return database[valueType];
		}


		public bool TryGet<TValue>(out TValue value)
		{
			value = default;

			bool result = database.TryGetValue(
				typeof(TValue),
				out var valueObject);

			if (result)
				value = (TValue)valueObject;

			return result;
		}

		public bool TryGet(
			Type valueType,
			out object value)
		{
			return database.TryGetValue(
				valueType,
				out value);
		}


		public int Count { get { return database.Count; } }

		public IEnumerable<Type> Keys => database.Keys;

		public IEnumerable<object> Values => database.Values;

		#endregion

		public void Add<TValue>(TValue value)
		{
			database.TryAdd(
				typeof(TValue),
				value);
		}

		public void Add(
			Type valueType,
			object value)
		{
			database.TryAdd(
				valueType,
				value);
		}


		public bool TryAdd<TValue>(TValue value)
		{
			return database.TryAdd(
				typeof(TValue),
				value);
		}

		public bool TryAdd(
			Type valueType,
			object value)
		{
			return database.TryAdd(
				valueType,
				value);
		}


		public void Update<TValue>(TValue value)
		{
			database[typeof(TValue)] = value;
		}

		public void Update(
			Type valueType,
			object value)
		{
			database[valueType] = value;
		}


		public bool TryUpdate<TValue>(TValue value)
		{
			return database.TryUpdate(
				typeof(TValue),
				value,
				database[typeof(TValue)]);
		}

		public bool TryUpdate(
			Type valueType,
			object value)
		{
			return database.TryUpdate(
				valueType,
				value,
				database[valueType]);
		}


		public void AddOrUpdate<TValue>(TValue value)
		{
			database.AddOrUpdate(
				typeof(TValue),
				value,
				(k, v) => value);
		}

		public void AddOrUpdate(
			Type valueType,
			object value)
		{
			database.AddOrUpdate(
				valueType,
				value,
				(k, v) => value);
		}


		public void Remove<TValue>()
		{
			database.TryRemove(
				typeof(TValue),
				out var value);
		}

		public void Remove(Type valueType)
		{
			database.TryRemove(
				valueType,
				out var value);
		}


		public bool TryRemove<TValue>()
		{
			return database.TryRemove(
				typeof(TValue),
				out var value);
		}

		public bool TryRemove(Type valueType)
		{
			return database.TryRemove(
				valueType,
				out var value);
		}


		public void Clear()
		{
			database.Clear();
		}

		#endregion

		#region ICloneableObjectRepository

		public IObjectRepository Clone()
		{
			return RepositoriesFactory.CloneConcurrentDictionaryObjectRepository(database);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			foreach (var value in database.Values)
			{
				if (value is ICleanUppable)
					(value as ICleanUppable).Cleanup();
			}

			Clear();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			foreach (var value in database.Values)
			{
				if (value is IDisposable)
					(value as IDisposable).Dispose();
			}

			Clear();

			if (database is IDisposable)
				(database as IDisposable).Dispose();
		}

		#endregion
	}
}