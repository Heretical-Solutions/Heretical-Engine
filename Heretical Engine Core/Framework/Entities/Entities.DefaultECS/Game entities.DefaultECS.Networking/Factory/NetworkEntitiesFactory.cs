using System.Reflection;
using DefaultEcs;

using HereticalSolutions.Logging;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.GameEntities.Factories
{
	/// <summary>
	/// Class containing methods to build entities and their components.
	/// </summary>
	public static partial class NetworkEntitiesFactory
	{
		public static EntityManager BuildSimpleNetworkEntityManager(
			IFormatLogger logger)
		{
			var registryEntityRepository = RepositoriesFactory.BuildDictionaryRepository<Guid, Entity>();

			var entityWorldsRepository = EntitiesFactory.BuildEntityWorldsRepository(logger);


			entityWorldsRepository.AddWorld(
				WorldConstants.REGISTRY_WORLD_ID,
				EntitiesFactory.BuildRegistryWorldController(
					EntitiesFactory.BuildPrototypesRepository(),
					logger));

			entityWorldsRepository.AddWorld(
				WorldConstants.EVENT_WORLD_ID,
				EntitiesFactory.BuildEventWorldController(
					logger));

			entityWorldsRepository.AddWorld(
				WorldConstants.SIMULATION_WORLD_ID,
				EntitiesFactory.BuildWorldController<SimulationEntityComponent, ResolveSimulationComponent>(
					(component) => { return component.SimulationEntity; },
					(component) => { return component.PrototypeID; },
					(prototypeID, entity) =>
					{
						return new SimulationEntityComponent
						{
							PrototypeID = prototypeID,

							SimulationEntity = entity
						};
					},
					(source) => { return new ResolveSimulationComponent { Source = source }; },
					logger));

			entityWorldsRepository.AddWorld(
				WorldConstants.VIEW_WORLD_ID,
				EntitiesFactory.BuildWorldController<ViewEntityComponent, ResolveViewComponent>(
					(component) => { return component.ViewEntity; },
					(component) => { return component.PrototypeID; },
					(prototypeID, entity) =>
					{
						return new ViewEntityComponent
						{
							PrototypeID = prototypeID,

							ViewEntity = entity
						};
					},
					(source) => { return new ResolveViewComponent { Source = source }; },
					logger));

			entityWorldsRepository.AddWorld(
				NetworkWorldConstants.NETWORKING_SERVER_DATA_WORLD_ID,
				EntitiesFactory.BuildWorldController<ServerDataEntityComponent, ResolveServerDataComponent>(
					(component) => { return component.ServerDataEntity; },
					(component) => { return component.PrototypeID; },
					(prototypeID, entity) =>
					{
						return new ServerDataEntityComponent
						{
							PrototypeID = prototypeID,

							ServerDataEntity = entity
						};
					},
					(source) => { return new ResolveServerDataComponent { Source = source }; },
					logger));

			entityWorldsRepository.AddWorld(
				NetworkWorldConstants.NETWORKING_PREDICTION_WORLD_ID,
				EntitiesFactory.BuildWorldController<PredictionEntityComponent, ResolvePredictionComponent>(
					(component) => { return component.PredictionEntity; },
					(component) => { return component.PrototypeID; },
					(prototypeID, entity) =>
					{
						return new PredictionEntityComponent
						{
							PrototypeID = prototypeID,

							PredictionEntity = entity
						};
					},
					(source) => { return new ResolvePredictionComponent { Source = source }; },
					logger));

			List<World> childEntityWorlds = new List<World>();

			childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID));
			childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.VIEW_WORLD_ID));
			childEntityWorlds.Add(entityWorldsRepository.GetWorld(NetworkWorldConstants.NETWORKING_SERVER_DATA_WORLD_ID));
			childEntityWorlds.Add(entityWorldsRepository.GetWorld(NetworkWorldConstants.NETWORKING_PREDICTION_WORLD_ID));

			return new EntityManager(
				registryEntityRepository,
				entityWorldsRepository,
				childEntityWorlds,
				logger);
		}

		public static NetworkEventEntityBuilder BuildEventEntityBuilder(
			World world)
		{
			return new NetworkEventEntityBuilder(world);
		}

		/// <summary>
		/// Builds an ECSWorldFullStateVisitor.
		/// </summary>
		/// <param name="entityManager">The entity manager to use.</param>
		/// <returns>The built ECSWorldFullStateVisitor.</returns>
		public static ECSWorldFullStateVisitor BuildECSWorldFullStateVisitor(
			IEntityManager<World, Entity> entityManager,
			IFormatLogger logger)
		{
			EntitiesFactory.BuildComponentTypesListWithAttribute<NetworkComponentAttribute>(
				out var componentTypes,
				out var hashToType,
				out var typeToHash);

			var readComponentMethodInfo =
				typeof(EntitiesFactory).GetMethod(
					"ReadComponent",
					BindingFlags.Static | BindingFlags.Public);

			var writeComponentMethodInfo =
				typeof(EntitiesFactory).GetMethod(
					"WriteComponent",
					BindingFlags.Static | BindingFlags.Public);

			return new ECSWorldFullStateVisitor(
				entityManager,
				hashToType,
				typeToHash,
				EntitiesFactory.BuildComponentReaders(
					readComponentMethodInfo,
					componentTypes),
				EntitiesFactory.BuildComponentWriters(
					writeComponentMethodInfo,
					componentTypes),
				logger);
		}

		public static ECSWorldMementoVisitor BuildECSWorldMementoVisitor(
			IEntityManager<World, Entity> entityManager,
			IFormatLogger logger)
		{
			EntitiesFactory.BuildComponentTypesListWithAttribute<NetworkComponentAttribute>(
				out var componentTypes,
				out var hashToType,
				out var typeToHash);

			var readComponentMethodInfo =
				typeof(EntitiesFactory).GetMethod(
					"ReadComponent",
					BindingFlags.Static | BindingFlags.Public);

			var writeComponentMethodInfo =
				typeof(EntitiesFactory).GetMethod(
					"WriteComponent",
					BindingFlags.Static | BindingFlags.Public);

			return new ECSWorldMementoVisitor(
				entityManager,
				hashToType,
				typeToHash,
				EntitiesFactory.BuildComponentReaders(
					readComponentMethodInfo,
					componentTypes),
				EntitiesFactory.BuildComponentWriters(
					writeComponentMethodInfo,
					componentTypes),
				new ECSWorldMemento(
					RepositoriesFactory.BuildDictionaryRepository<Guid, ECSEntityMemento>(),
					new List<ECSEntityCreatedDeltaDTO>(),
					new List<ECSEntityDestroyedDeltaDTO>(),
					new List<ECSComponentDeltaDTO>(),
					new List<ECSComponentDeltaDTO>()),
				new HashSet<Guid>(),
				new List<ECSComponentDTO>(),
				logger);
		}

		public static ECSEventWorldVisitor BuildECSEventWorldVisitor(
			IEventEntityBuilder<Entity> eventEntityBuilder,
			bool host,
			IFormatLogger logger)
		{
			EntitiesFactory.BuildComponentTypesListWithAttribute<NetworkEventComponentAttribute>(
				out var componentTypes,
				out var hashToType,
				out var typeToHash);

			var readComponentMethodInfo =
				typeof(EntitiesFactory).GetMethod(
					"ReadComponent",
					BindingFlags.Static | BindingFlags.Public);

			var writeComponentMethodInfo =
				typeof(EntitiesFactory).GetMethod(
					"WriteComponent",
					BindingFlags.Static | BindingFlags.Public);

			return new ECSEventWorldVisitor(
				eventEntityBuilder,
				hashToType,
				typeToHash,
				EntitiesFactory.BuildComponentReaders(
					readComponentMethodInfo,
					componentTypes),
				EntitiesFactory.BuildComponentWriters(
					writeComponentMethodInfo,
					componentTypes),
				host,
				logger);
		}
	}
}