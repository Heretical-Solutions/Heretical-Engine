using DefaultEcs;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Logging;

using HereticalSolutions.Repositories;

using Entity = DefaultEcs.Entity;

namespace HereticalSolutions.GameEntities
{
    public class EntityManager
        : IEntityManager,
          IEntityManagerInternal
    {
        private readonly IRepository<Guid, Entity> entityRepository;
        
        private readonly IRepository<string, Entity> prototypesRepository;

        private readonly IRepository<EWorld, World> worldsRepository;
        
        private readonly IRepository<EWorld, IWorldController> worldControllersRepository;

        private readonly IFormatLogger logger;

        public EntityManager(
            IRepository<Guid, Entity> entityRepository,
            IRepository<string, Entity> prototypesRepository,
            IRepository<EWorld, World> worldsRepository,
            IRepository<EWorld, IWorldController> worldControllersRepository,
            IEventEntityBuilder eventEntityBuilder,
            IMultiWorldSetter multiWorldSetter,
            IFormatLogger logger)
        {
            this.entityRepository = entityRepository;

            this.prototypesRepository = prototypesRepository;

            this.worldsRepository = worldsRepository;
            
            this.worldControllersRepository = worldControllersRepository;

            EventEntityBuilder = eventEntityBuilder;

            MultiWorldSetter = multiWorldSetter;

            this.logger = logger;

            foreach (var world in worldControllersRepository.Keys)
            {
                ((IWorldControllerInternal)this.worldControllersRepository.Get(world)).InitializeEntityManagerInternal(this);
            }
        }

        #region IEntityManagerInternal

        public IRepository<string, Entity> PrototypesRepository
        {
            get => prototypesRepository;
        }
        
        public IRepository<EWorld, World> WorldsRepository
        {
            get => worldsRepository;
        }
        
        public IRepository<EWorld, IWorldController> WorldControllersRepository
        {
            get => worldControllersRepository;
        }
        
        #endregion

        #region IWorldRepository

        public World GetWorld(EWorld worldID)
        {
            if (!worldsRepository.TryGet(worldID, out var targetWorld))
            {
                logger.LogError<EntityManager>(
                    $"NO WORLD REGISTERED BY ID {worldID.ToString()}");
                
                return null;
            }

            return targetWorld;
        }

        public IWorldController GetWorldController(EWorld worldID)
        {
            if (!worldControllersRepository.TryGet(worldID, out var targetWorld))
            {
                logger.LogError<EntityManager>(
                    $"NO WORLD CONTROLLER REGISTERED BY ID {worldID.ToString()}");
                
                return null;
            }

            return targetWorld;
        }
        
        #endregion

        #region IPrototypeRepository

        public bool HasPrototype(string prototypeID)
        {
            return prototypesRepository.Has(prototypeID);
        }

        public Entity GetPrototype(string prototypeID)
        {
            if (!prototypesRepository.TryGet(prototypeID, out var prototypeEntity))
            {
                logger.LogError<EntityManager>(
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return default(Entity);
            }

            return prototypeEntity;
        }

        public void AddPrototype(
            string prototypeID,
            Entity prototypeEntity)
        {
            prototypesRepository.TryAdd(
                prototypeID,
                prototypeEntity);
        }

        public void RemovePrototype(string prototypeID)
        {
            prototypesRepository.TryRemove(prototypeID);
        }

        #endregion

        #region IEntityRepository

        public bool HasEntity(Guid guid)
        {
            return entityRepository.Has(guid);
        }

        public Entity GetEntity(Guid guid)
        {
            if (!entityRepository.TryGet(guid, out var result))
                return default(Entity);

            return result;
        }

        public GuidPrototypeIDPair[] AllEntities
        {
            get
            {
                var keys = entityRepository.Keys;
                
                var result = new GuidPrototypeIDPair[keys.Count()];

                int index = 0;
                
                foreach (var key in keys)
                {
                    result[index] = new GuidPrototypeIDPair
                    {
                        GUID = key,
                        
                        PrototypeID = entityRepository.Get(key).Get<RegistryEntityComponent>().PrototypeID
                    };
                }

                return result;
            }
        }
        
        public IEnumerable<Guid> GetAllGUIDs => entityRepository.Keys;

        #endregion

        #region IContainsEventEntityBuilder

        public IEventEntityBuilder EventEntityBuilder { get; private set; }

        #endregion

        #region IContainsMultiWorldSetter

        public IMultiWorldSetter MultiWorldSetter { get; private set; }

        #endregion

        #region IEntityManager

        #region Spawn entity
        
        public Guid SpawnEntity(
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE)
        {
            var newGUID = IDAllocationsFactory.BuildGUID();

            if (!SpawnEntity(
                    newGUID,
                    prototypeID,
                    authoring))
                return default(Guid);

            return newGUID;
        }
        
        public void SpawnEntityFromServer(
            Guid guid,
            string prototypeID)
        {
            SpawnEntity(
                guid,
                prototypeID,
                EEntityAuthoring.NETWORKING_CLIENT);
        }

        private bool SpawnEntity(
            Guid guid,
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE)
        {
            IWorldControllerInternal registryWorld = (IWorldControllerInternal)worldControllersRepository.Get(EWorld.REGISTRY);
            
            if (!registryWorld.SpawnEntity(
                    prototypeID,
                    guid,
                    out var entity))
            {
                return false;
            }

            entityRepository.Add(
                guid,
                entity);
            
            switch (authoring)
            {
                case EEntityAuthoring.OFFLINE:
                    
                    RemoveEntityComponentFromRegistry<ServerEntityComponent>(entity);
                    
                    RemoveEntityComponentFromRegistry<PredictionEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SIMULATION)).SpawnEntityFromPrototype(
                        entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.VIEW)).SpawnEntityFromPrototype(
                        entity);

                    break;
                
                case EEntityAuthoring.NETWORKING_HOST:
                    
                    RemoveEntityComponentFromRegistry<PredictionEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SIMULATION)).SpawnEntityFromPrototype(
                        entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SERVER)).SpawnEntityFromPrototype(
                        entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.VIEW)).SpawnEntityFromPrototype(
                        entity);

                    break;
                
                case EEntityAuthoring.NETWORKING_HOST_HEADLESS:
                    
                    RemoveEntityComponentFromRegistry<PredictionEntityComponent>(entity);
                    
                    RemoveEntityComponentFromRegistry<ViewEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SIMULATION)).SpawnEntityFromPrototype(
                        entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SERVER)).SpawnEntityFromPrototype(
                        entity);

                    break;
                
                case EEntityAuthoring.NETWORKING_CLIENT:
                    
                    RemoveEntityComponentFromRegistry<SimulationEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SERVER)).SpawnEntityFromPrototype(
                        entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.PREDICTION)).SpawnEntityFromPrototype(
                        entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.VIEW)).SpawnEntityFromPrototype(
                        entity);
                    
                    break;
            }

            return true;
        }

        private void RemoveEntityComponentFromRegistry<TEntity>(Entity registryEntity)
        {
            if (registryEntity.Has<TEntity>())
                registryEntity.Remove<TEntity>();
        }

        #endregion

        #region Resolve entity

        public void ResolveEntity(
            object target,
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE)
        {
            var newGUID = IDAllocationsFactory.BuildGUID();

            SpawnAndResolveEntity(
                target,
                newGUID,
                prototypeID,
                authoring);
        }
        
        public void ResolveEntity(
            Guid guid,
            object target,
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE)
        {
            SpawnAndResolveEntity(
                target,
                guid,
                prototypeID,
                authoring);
        }
        
        private bool SpawnAndResolveEntity(
            object target,
            Guid guid,
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE)
        {
            IWorldControllerInternal registryWorld = (IWorldControllerInternal)worldControllersRepository.Get(EWorld.REGISTRY); 
            
            if (!registryWorld.SpawnEntity(
                    prototypeID,
                    guid,
                    out var entity))
            {
                return false;
            }

            entityRepository.Add(
                guid,
                entity);
            
            switch (authoring)
            {
                case EEntityAuthoring.OFFLINE:
                    
                    RemoveEntityComponentFromRegistry<ServerEntityComponent>(entity);
                    
                    RemoveEntityComponentFromRegistry<PredictionEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SIMULATION)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.VIEW)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);

                    break;
                
                case EEntityAuthoring.NETWORKING_HOST:
                    
                    RemoveEntityComponentFromRegistry<PredictionEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SIMULATION)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SERVER)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.VIEW)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);
                    
                    break;
                
                case EEntityAuthoring.NETWORKING_HOST_HEADLESS:
                    
                    RemoveEntityComponentFromRegistry<PredictionEntityComponent>(entity);
                    
                    RemoveEntityComponentFromRegistry<ViewEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SIMULATION)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SERVER)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);

                    break;
                
                case EEntityAuthoring.NETWORKING_CLIENT:
                    
                    RemoveEntityComponentFromRegistry<SimulationEntityComponent>(entity);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.SERVER)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.PREDICTION)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);
                    
                    ((IWorldControllerInternal)worldControllersRepository.Get(EWorld.VIEW)).SpawnAndResolveEntityFromPrototype(
                        entity,
                        target);

                    break;
            }

            return true;
        }

        #endregion
        
        #region Despawn entity
        
        public void DespawnEntity(Guid guid)
        {
            var registryEntity = entityRepository.Get(guid);
        
            worldControllersRepository.Get(EWorld.SIMULATION).DespawnEntity(registryEntity);
            
            worldControllersRepository.Get(EWorld.SERVER).DespawnEntity(registryEntity);
            
            worldControllersRepository.Get(EWorld.PREDICTION).DespawnEntity(registryEntity);
            
            worldControllersRepository.Get(EWorld.VIEW).DespawnEntity(registryEntity);
            
            registryEntity.Set<DespawnComponent>();
            
            entityRepository.Remove(
                guid);
        }

        #endregion
        
        #endregion
    }
}