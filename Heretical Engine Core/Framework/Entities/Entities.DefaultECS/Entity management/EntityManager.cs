using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

using World = DefaultEcs.World;

using Entity = DefaultEcs.Entity;

using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    public class EntityManager
        : IEntityManager<World, Entity>
    {
        private readonly IRepository<Guid, Entity> registryEntitiesRepository;

        private readonly IReadOnlyEntityWorldsRepository<World, ISystem<Entity>, Entity> entityWorldsRepository;

        //TODO: ensure that it's what this class needs
        private readonly IReadOnlyList<World> childEntityWorlds;

        private readonly IFormatLogger logger;

        public EntityManager(
            IRepository<Guid, Entity> registryEntitiesRepository,
            IReadOnlyEntityWorldsRepository<World, ISystem<Entity>, Entity> entityWorldsRepository,
            IReadOnlyList<World> childEntityWorlds,
            IFormatLogger logger = null)
        {
            this.registryEntitiesRepository = registryEntitiesRepository;

            this.entityWorldsRepository = entityWorldsRepository;

            this.childEntityWorlds = childEntityWorlds;

            this.logger = logger;
        }

        #region IEntityManager

        #region IReadOnlyEntityRepository

        public bool HasEntity(Guid guid)
        {
            return registryEntitiesRepository.Has(guid);
        }

        public Entity GetRegistryEntity(Guid guid)
        {
            if (!registryEntitiesRepository.TryGet(
                guid,
                out var result))
                return default(Entity);

            return result;
        }

        public GuidPrototypeIDPair[] AllRegistryEntities
        {
            get
            {
                var keys = registryEntitiesRepository.Keys;
                
                var result = new GuidPrototypeIDPair[keys.Count()];

                int index = 0;
                
                foreach (var key in keys)
                {
                    result[index] = new GuidPrototypeIDPair
                    {
                        GUID = key,
                        
                        PrototypeID = registryEntitiesRepository.Get(key).Get<RegistryEntityComponent>().PrototypeID
                    };
                }

                return result;
            }
        }
        
        public IEnumerable<Guid> AllAllocatedGUIDs => registryEntitiesRepository.Keys;

        #endregion

        #region Spawn entity
        
        public Guid SpawnEntity(
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var newGUID = AllocateGUID();

            if (!SpawnEntityInAllRelevantWorlds(
                    newGUID,
                    prototypeID,
                    authoringPreset))
                return default(Guid);

            return newGUID;
        }

        public bool SpawnEntity(
            Guid guid,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            return SpawnEntityInAllRelevantWorlds(
                guid,
                prototypeID,
                authoringPreset);
        }

        public Entity SpawnWorldLocalEntity(
            string prototypeID,
            string worldID)
        {
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(worldID);

            if (worldController == null)
                return default;

            worldController.TrySpawnEntityFromPrototype(
                prototypeID,
                out var localEntity);

            return localEntity;
        }

        public Entity SpawnWorldLocalEntity(
            string prototypeID,
            World world)
        {
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(world);

            if (worldController == null)
                return default;

            worldController.TrySpawnEntityFromPrototype(
                prototypeID,
                out var localEntity);

            return localEntity;
        }

        #endregion

        #region Resolve entity

        public Guid ResolveEntity(
            object source,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var newGUID = AllocateGUID();

            if (!SpawnAndResolveEntityInAllRelevantWorlds(
                newGUID,
                prototypeID,
                authoringPreset))
                return default(Guid);

            return newGUID;
        }

        public bool ResolveEntity(
            Guid guid,
            object source,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            return SpawnAndResolveEntityInAllRelevantWorlds(
                guid,
                prototypeID,
                source,
                authoringPreset);
        }

        public Entity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            string worldID)
        {
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(worldID);

            if (worldController == null)
                return default;

            worldController.TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out var localEntity);

            return localEntity;
        }

        public Entity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            World world)
        {
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(world);

            if (worldController == null)
                return default;

            worldController.TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out var localEntity);

            return localEntity;
        }

        #endregion

        #region Despawn entity

        public void DespawnEntity(Guid guid)
        {
            if (!registryEntitiesRepository.TryGet(
                guid,
                out var registryEntity))
            {
                logger?.LogError<EntityWorldsRepository>(
                    $"NO ENTITY REGISTERED BY GUID {guid}");

                return;
            }

            foreach (var entityWorld in childEntityWorlds)
            {
                var worldController = (IRegistryCompliantWorldController<Entity>)
                    entityWorldsRepository.GetWorldController(entityWorld);

                if (worldController == null)
                    continue;

                worldController.DespawnEntityAndUnlinkFromRegistry(
                    registryEntity);
            }

            var registryWorldController =
                entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);

            registryWorldController.DespawnEntity(
                registryEntity);

            registryEntitiesRepository.Remove(
                guid);
        }

        public void DespawnWorldLocalEntity(Entity entity)
        {
            if (entity == default)
                return;

            var world = entity.World;

            var worldController = entityWorldsRepository.GetWorldController(world);

            worldController.DespawnEntity(
                entity);
        }

        #endregion

        #endregion

        private Guid AllocateGUID()
        {
            Guid newGUID;

            do
            {
                newGUID = IDAllocationsFactory.BuildGUID();
            }
            while (registryEntitiesRepository.Has(newGUID));

            return newGUID;
        }

        /*
        public void SpawnEntityFromServer(
            Guid guid,
            string prototypeID)
        {
            SpawnEntity(
                guid,
                prototypeID,
                EEntityAuthoringPresets.NETWORKING_CLIENT);
        }
        */

        private bool SpawnEntityInAllRelevantWorlds(
            Guid guid,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var registryWorldController = (IGUIDCompliantWorldController<Entity>)
                entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);
            
            if (!registryWorldController.TrySpawnEntityWithGUIDFromPrototype(
                    prototypeID,
                    guid,
                    out var registryEntity))
            {
                return false;
            }

            registryEntitiesRepository.Add(
                guid,
                registryEntity);
            
            switch (authoringPreset)
            {
                case EEntityAuthoringPresets.DEFAULT:
                    foreach (var entityWorld in childEntityWorlds)
                    {
                        var worldController = (IRegistryCompliantWorldController<Entity>)
                            entityWorldsRepository.GetWorldController(entityWorld);

                        if (worldController == null)
                            continue;

                        worldController.TrySpawnEntityFromRegistry(
                            registryEntity,
                            out var localEntity);
                    }

                    break;
                
                default:
                    break;
            }

            return true;
        }

        /*
        private void RemoveEntityComponentFromRegistry<TEntity>(Entity registryEntity)
        {
            if (registryEntity.Has<TEntity>())
                registryEntity.Remove<TEntity>();
        }
        */

        private bool SpawnAndResolveEntityInAllRelevantWorlds(
            Guid guid,
            string prototypeID,
            object source,
            EEntityAuthoringPresets authoring = EEntityAuthoringPresets.DEFAULT)
        {
            var registryWorldController = (IGUIDCompliantWorldController<Entity>)
                entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);

            if (!registryWorldController.TrySpawnEntityWithGUIDFromPrototype(
                    prototypeID,
                    guid,
                    out var registryEntity))
            {
                return false;
            }

            registryEntitiesRepository.Add(
                guid,
                registryEntity);
            
            switch (authoring)
            {
                case EEntityAuthoringPresets.DEFAULT:
                    foreach (var entityWorld in childEntityWorlds)
                    {
                        var worldController = (IRegistryCompliantWorldController<Entity>)
                            entityWorldsRepository.GetWorldController(entityWorld);

                        if (worldController == null)
                            continue;

                        worldController.TrySpawnAndResolveEntityFromRegistry(
                            registryEntity,
                            source,
                            out var localEntity);
                    }

                    break;
                
                default:
                    break;
            }

            return true;
        }
    }
}