using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence;

using HereticalSolutions.GameEntities.Factories;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.Serialization;

namespace HereticalSolutions.GameEntities
{
	public class EntityPrototypeVisitor
		: ILoadVisitorGeneric<Entity, EntityPrototypeDTO>,
		  ILoadVisitor,
		  ISaveVisitorGeneric<Entity, EntityPrototypeDTO>,
		  ISaveVisitor
	{
		#region Reflection

		private static Type[] componentTypes;

		private static MethodInfo writeComponentMethodInfo;

		private static IReadOnlyRepository<Type, EntityFactoryWriteComponentDelegate> componentWriters;

		//private static Type[] viewComponentTypes;

		//private static MethodInfo addComponentMethodInfo;

		//private static IReadOnlyRepository<Type, EntityFactoryAddComponentDelegate> componentAdders;

		#endregion

		private readonly IPrototypesRepository<World, Entity> prototypesRepository;

		private readonly IFormatLogger logger;

		public EntityPrototypeVisitor(
			IPrototypesRepository<World, Entity> prototypesRepository,
			IFormatLogger logger)
		{
			this.prototypesRepository = prototypesRepository;

			this.logger = logger;
		}

		#region ILoadVisitorGeneric

		public bool Load(
			EntityPrototypeDTO DTO,
			out Entity value)
		{
			LazyInitialization();

			prototypesRepository.TryAllocatePrototype(
				DTO.PrototypeID,
				out value);

			foreach (var component in DTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						value,
						component);
			}

			return true;
		}

		public bool Load(
			EntityPrototypeDTO DTO,
			Entity valueToPopulate)
		{
			LazyInitialization();

			foreach (var component in DTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						valueToPopulate,
						component);
			}

			return true;
		}

		#endregion

		#region ILoadVisitor

		public bool Load<TValue>(object DTO, out TValue value)
		{
			if (!(DTO.GetType().Equals(typeof(EntityPrototypeDTO))))
				logger.ThrowException<EntityPrototypeVisitor>(
					$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(EntityPrototypeDTO)}\" RECEIVED: \"{DTO.GetType()}\"");

			bool result = Load((
				EntityPrototypeDTO)DTO,
				out Entity returnValue);

			// DIRTY HACKS DO NOT REPEAT
			value = result
				? (TValue)(object)returnValue
				: default;

			return result;
		}

		public bool Load<TValue, TDTO>(TDTO DTO, out TValue value)
		{
			if (!(typeof(TDTO).Equals(typeof(EntityPrototypeDTO))))
				logger.ThrowException<EntityPrototypeVisitor>(
					$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(EntityPrototypeDTO)}\" RECEIVED: \"{typeof(TDTO)}\"");

			// DIRTY HACKS DO NOT REPEAT
			var dtoObject = (object)DTO;

			bool result = Load(
				(EntityPrototypeDTO)dtoObject,
				out Entity returnValue);

			// DIRTY HACKS DO NOT REPEAT
			value = result
				? (TValue)(object)returnValue
				: default;

			return result;
		}

		public bool Load<TValue>(object DTO, TValue valueToPopulate)
		{
			if (!(DTO.GetType().Equals(typeof(EntityPrototypeDTO))))
				logger.ThrowException<EntityPrototypeVisitor>(
					$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(EntityPrototypeDTO)}\" RECEIVED: \"{DTO.GetType()}\"");

			return Load(
				(EntityPrototypeDTO)DTO,
				(Entity)(object)valueToPopulate); // DIRTY HACKS DO NOT REPEAT
		}

		public bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)
		{
			if (!(typeof(TDTO).Equals(typeof(EntityPrototypeDTO))))
				logger.ThrowException<EntityPrototypeVisitor>(
					$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(EntityPrototypeDTO)}\" RECEIVED: \"{typeof(TDTO)}\"");

			// DIRTY HACKS DO NOT REPEAT
			var dtoObject = (object)DTO;

			return Load(
				(EntityPrototypeDTO)dtoObject,
				(Entity)(object)valueToPopulate); // DIRTY HACKS DO NOT REPEAT
		}

		#endregion

		#region ISaveVisitorGeneric

		public bool Save(Entity value, out EntityPrototypeDTO DTO)
		{
			var entitySerializationWrapper = new EntitySerializationWrapper(value);

			object[] componentsArray = new object[entitySerializationWrapper.Components.Length];

			for (int i = 0; i < componentsArray.Length; i++)
			{
				componentsArray[i] = entitySerializationWrapper.Components[i].ObjectValue;
			}

			string prototypeID = string.Empty;

			//TODO: optimize
			foreach (var key in prototypesRepository.AllPrototypeIDs)
			{
				if (prototypesRepository.TryGetPrototype(
					key,
					out Entity prototypeEntity))
				{
					if (prototypeEntity.Equals(value))
					{
						prototypeID = key;

						break;
					}
				}
			}

			DTO = new EntityPrototypeDTO
			{
				PrototypeID = prototypeID,

				Components = componentsArray
			};

			return true;
		}

		#endregion

		#region ISaveVisitor

		public bool Save<TValue>(TValue value, out object DTO)
		{
			if (!(typeof(Entity).IsAssignableFrom(typeof(TValue))))
				logger.ThrowException<EntityPrototypeVisitor>(
					$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(Entity)}\" RECEIVED: \"{typeof(TValue)}\"");

			bool result = Save(
				(Entity)(object)value, // DIRTY HACKS DO NOT REPEAT
				out EntityPrototypeDTO returnDTO);

			DTO = result
				? returnDTO
				: default(object);

			return result;
		}
		public bool Save<TValue, TDTO>(TValue value, out TDTO DTO)
		{
			if (!(typeof(Entity).IsAssignableFrom(typeof(TValue))))
				logger.ThrowException<EntityPrototypeVisitor>(
					$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(Entity)}\" RECEIVED: \"{typeof(TValue)}\"");

			bool result = Save(
				(Entity)(object)value, // DIRTY HACKS DO NOT REPEAT
				out EntityPrototypeDTO returnDTO);

			if (result)
			{
				// DIRTY HACKS DO NOT REPEAT
				var dtoObject = (object)returnDTO;

				DTO = (TDTO)dtoObject;
			}
			else
			{
				DTO = default(TDTO);
			}

			return result;
		}

		#endregion

		private void LazyInitialization()
		{
			if (componentTypes == null)
			{
				EntitiesFactory.BuildComponentTypesListWithAttribute<ComponentAttribute>(
					out componentTypes);
			}

			if (writeComponentMethodInfo == null)
			{
				writeComponentMethodInfo =
					typeof(EntityPrototypeVisitor).GetMethod(
						"WriteComponent",
						BindingFlags.Static | BindingFlags.Public);
			}

			if (componentWriters == null)
			{
				componentWriters = BuildComponentWriters(
					writeComponentMethodInfo,
					componentTypes);
			}
		}

		private static IReadOnlyRepository<Type, EntityFactoryAddComponentDelegate> BuildComponentAdders(
			MethodInfo addComponentMethodInfo,
			Type[] viewComponentTypes)
		{
			IReadOnlyRepository<Type, EntityFactoryAddComponentDelegate> result =
				RepositoriesFactory.BuildDictionaryRepository<Type, EntityFactoryAddComponentDelegate>();

			for (int i = 0; i < viewComponentTypes.Length; i++)
			{
				MethodInfo addComponentGeneric = addComponentMethodInfo.MakeGenericMethod(viewComponentTypes[i]);

				EntityFactoryAddComponentDelegate addComponentGenericDelegate =
					(EntityFactoryAddComponentDelegate)addComponentGeneric.CreateDelegate(
						typeof(EntityFactoryAddComponentDelegate),
						null);

				((IRepository<Type, EntityFactoryAddComponentDelegate>)result).Add(
					viewComponentTypes[i],
					addComponentGenericDelegate);
			}

			return result;
		}

		public static void AddComponent<TComponent>(
			Entity entity,
			object component)
		{
			// Early return for AoT compilation calls
			if (component == null)
				return;

			entity.Set<TComponent>((TComponent)component);
		}

		private static IReadOnlyRepository<Type, EntityFactoryWriteComponentDelegate> BuildComponentWriters(
			MethodInfo writeComponentMethodInfo,
			Type[] componentTypes)
		{
			IReadOnlyRepository<Type, EntityFactoryWriteComponentDelegate> result =
				RepositoriesFactory.BuildDictionaryRepository<Type, EntityFactoryWriteComponentDelegate>();

			for (int i = 0; i < componentTypes.Length; i++)
			{
				MethodInfo writeComponentGeneric = writeComponentMethodInfo.MakeGenericMethod(componentTypes[i]);

				EntityFactoryWriteComponentDelegate writeComponentGenericDelegate =
					(EntityFactoryWriteComponentDelegate)writeComponentGeneric.CreateDelegate(
						typeof(EntityFactoryWriteComponentDelegate),
						null);

				((IRepository<Type, EntityFactoryWriteComponentDelegate>)result).Add(
					componentTypes[i],
					writeComponentGenericDelegate);
			}

			return result;
		}

		public static void WriteComponent<TComponent>(
			Entity entity,
			object componentValue)
		{
			// Early return for AoT compilation calls
			if (componentValue == null)
				return;

			entity.Set<TComponent>((TComponent)componentValue);
		}

		interface IComponentWrapper
		{
			Type Type { get; }

			object ObjectValue { get; }
		}

		private class EntitySerializationWrapper
		{
			private class ComponentWrapper<T> : IComponentWrapper
			{
				public bool IsEnabled { get; }

				public T Value { get; }

				public object ObjectValue
				{
					get => Value;
				}

				public Type Type => typeof(T);

				public ComponentWrapper(bool isEnabled, T value)
				{
					Value = value;

					IsEnabled = isEnabled;
				}
			}

			private sealed class ComponentSerializationReader : IComponentReader
			{
				private readonly Entity _entity;

				private readonly List<IComponentWrapper> _components;

				public ComponentSerializationReader(in Entity entity, List<IComponentWrapper> components)
				{
					_components = components;
					_entity = entity;
				}

				public void OnRead<T>(in T component, in Entity componentOwner) => _components.Add(new ComponentWrapper<T>(_entity.IsEnabled<T>(), component));
			}

			private readonly Entity _entity;
			private readonly List<IComponentWrapper> _components;

			public World World => _entity.World;
			public bool IsAlive => _entity.IsAlive;
			public bool IsEnabled => _entity.IsEnabled();
			public IComponentWrapper[] Components => _components.ToArray();

			public EntitySerializationWrapper(Entity entity)
			{
				_entity = entity;

				_components = new List<IComponentWrapper>();

				entity.ReadAllComponents(new ComponentSerializationReader(_entity, _components));

				_components.Sort((o1, o2) => string.Compare(o1.Type.FullName, o2.Type.FullName));
			}
		}
	}
}