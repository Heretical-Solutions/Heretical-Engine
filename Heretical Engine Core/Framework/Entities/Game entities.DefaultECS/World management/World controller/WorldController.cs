using DefaultEcs;
using DefaultEcs.System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.GameEntities
{
    public class WorldController<TEntityIdentityComponent, TResolveComponent>
        : IWorldController<World, ISystem<Entity>, Entity>,
          IGUIDCompliantWorldController<Entity>,
          IRegistryCompliantWorldController<Entity>
    {
        #region Delegates

        private readonly Func<TEntityIdentityComponent, Entity> getEntityFromIdentityComponentDelegate;

        private readonly Func<TEntityIdentityComponent, string> getPrototypeIDFromIdentityComponentDelegate;

        private readonly Func<string, Entity, TEntityIdentityComponent> setIdentityComponentValuesDelegate;

        private readonly Func<object, TResolveComponent> createResolveComponentDelegate;

        #endregion

        private readonly IPrototypesRepository<Entity> prototypeRepository;

        #region Systems

        private ISystem<Entity> resolveSystems;

        private ISystem<Entity> initializationSystems;

        private ISystem<Entity> deinitializationSystems;

        #endregion

        private readonly IFormatLogger logger;
        

        public WorldController(
            World world,
            Func<TEntityIdentityComponent, Entity> getEntityFromIdentityComponentDelegate,
            Func<TEntityIdentityComponent, string> getPrototypeIDFromIdentityComponentDelegate,
            Func<string, Entity, TEntityIdentityComponent> setIdentityComponentValuesDelegate,
            Func<object, TResolveComponent> createResolveComponentDelegate,
            IPrototypesRepository<Entity> prototypeRepository,
            IFormatLogger logger)
        {
            World = world;

            this.getEntityFromIdentityComponentDelegate = getEntityFromIdentityComponentDelegate;

            this.getPrototypeIDFromIdentityComponentDelegate = getPrototypeIDFromIdentityComponentDelegate;

            this.setIdentityComponentValuesDelegate = setIdentityComponentValuesDelegate;

            this.createResolveComponentDelegate = createResolveComponentDelegate;

            this.prototypeRepository = prototypeRepository;

            this.logger = logger;


            resolveSystems = null;

            initializationSystems = null;

            deinitializationSystems = null;
        }

        public void Initialize(
            ISystem<Entity> resolveSystems,
            ISystem<Entity> initializationSystems,
            ISystem<Entity> deinitializationSystems)
        {
            this.resolveSystems = resolveSystems;

            this.initializationSystems = initializationSystems;

            this.deinitializationSystems = deinitializationSystems;
        }

        #region IWorldController

        public World World { get; private set; }


        public IPrototypesRepository<Entity> PrototypeRepository { get; }


        //Entity systems
        public ISystem<Entity> EntityResolveSystems { get => resolveSystems; }

        public ISystem<Entity> EntityInitializationSystems { get => initializationSystems; }

        public ISystem<Entity> EntityDeinitializationSystems { get => deinitializationSystems; }


        public bool TrySpawnEntityFromPrototype(
            string prototypeID,
            out Entity entity)
        {
            if (!TrySpawnEntity(
                prototypeID,
                out entity))
            {
                return false;
            }

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototype(
            string prototypeID,
            object source,
            out Entity entity)
        {
            if (!TrySpawnEntity(
                prototypeID,
                out entity))
            {
                return false;
            }

            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveComponent>(
                createResolveComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveComponent>();

            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public void DespawnEntity(
            Entity entity)
        {
            if (entity == default)
                return;

            if (entity.World != World)
                logger?.LogError<RegistryWorldController>(
                    $"ATTEMPT TO DESPAWN ENTITY FROM THE WRONG WORLD");

            if (entity.Has<DespawnComponent>())
                return;

            //Mark the entity for despawn
            entity.Set<DespawnComponent>();

            //Process the entity on its way to be despawned with deinitialization systems
            deinitializationSystems?.Update(entity);
        }

        #endregion

        #region IGUIDCompliantWorldController

        public bool TrySpawnEntityWithGUIDFromPrototype(
            string prototypeID,
            Guid guid,
            out Entity entity)
        {
            if (!TrySpawnEntityFromPrototype(
                prototypeID,
                out entity))
            {
                return false;
            }

            //Give the entity its GUID
            ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();

            guidComponent.GUID = guid;


            return true;
        }

        public bool TrySpawnAndResolveEntityWithGUIDFromPrototype(
            string prototypeID,
            Guid guid,
            object source,
            out Entity entity)
        {
            if (!TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out entity))
            {
                return false;
            }

            //Give the entity its GUID
            ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();

            guidComponent.GUID = guid;


            return true;
        }

        #endregion

        #region IRegistryCompliantWorldController

        public bool TrySpawnEntityFromRegistry(
            Entity registryEntity,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TEntityIdentityComponent>())
            {
                return false;
            }

            //Get the target GUID from the registry entity
            var guid = registryEntity.Get<GUIDComponent>().GUID;


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TEntityIdentityComponent>();

            var prototypeID = getPrototypeIDFromIdentityComponentDelegate.Invoke(entityIdentityComponent);


            if (!TrySpawnEntityWithGUIDFromPrototype(
                prototypeID,
                guid,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TEntityIdentityComponent>(
                setIdentityComponentValuesDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnAndResolveEntityFromRegistry(
            Entity registryEntity,
            object source,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TEntityIdentityComponent>())
            {
                return false;
            }

            //Get the target GUID from the registry entity
            var guid = registryEntity.Get<GUIDComponent>().GUID;


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TEntityIdentityComponent>();

            var prototypeID = getPrototypeIDFromIdentityComponentDelegate.Invoke(entityIdentityComponent);


            if (!TrySpawnAndResolveEntityWithGUIDFromPrototype(
                prototypeID,
                guid,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TEntityIdentityComponent>(
                setIdentityComponentValuesDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnEntityFromPrototypeAndLinkToRegistry(
            Entity registryEntity,
            string prototypeID,
            out Entity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            if (registryEntity.Has<TEntityIdentityComponent>())
            {
                return false;
            }

            //Get the target GUID from the registry entity
            var guid = registryEntity.Get<GUIDComponent>().GUID;

            if (!TrySpawnEntityWithGUIDFromPrototype(
                prototypeID,
                guid,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TEntityIdentityComponent>(
                setIdentityComponentValuesDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototypeAndLinkToRegistry(
            Entity registryEntity,
            string prototypeID,
            object source,
            out Entity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            if (registryEntity.Has<TEntityIdentityComponent>())
            {
                return false;
            }

            //Get the target GUID from the registry entity
            var guid = registryEntity.Get<GUIDComponent>().GUID;

            if (!TrySpawnAndResolveEntityWithGUIDFromPrototype(
                prototypeID,
                guid,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TEntityIdentityComponent>(
                setIdentityComponentValuesDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public void DespawnEntityAndUnlinkFromRegistry(
            Entity registryEntity)
        {
            if (!registryEntity.Has<TEntityIdentityComponent>())
                return;

            ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();

            var localEntity = getEntityFromIdentityComponentDelegate.Invoke(entityIdentityComponent);

            DespawnEntity(localEntity);

            registryEntity.Remove<TEntityIdentityComponent>();
        }

        public bool TryReplaceEntityFromPrototypeAndUpdateRegistry(
            Entity registryEntity,
            string prototypeID,
            out Entity localEntity)
        {
            bool alreadyHasIdentityComponent = registryEntity.Has<TEntityIdentityComponent>();

            if (alreadyHasIdentityComponent)
            {
                ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();

                var previousEntity = getEntityFromIdentityComponentDelegate.Invoke(entityIdentityComponent);

                DespawnEntity(previousEntity);
            }

            //Get the target GUID from the registry entity
            var guid = registryEntity.Get<GUIDComponent>().GUID;

            if (!TrySpawnEntityWithGUIDFromPrototype(
                prototypeID,
                guid,
                out localEntity))
            {
                registryEntity.Remove<TEntityIdentityComponent>();

                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TEntityIdentityComponent>(
                setIdentityComponentValuesDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TryReplaceAndResolveEntityFromPrototypeAndUpdateRegistry(
            Entity registryEntity,
            string prototypeID,
            object source,
            out Entity localEntity)
        {
            bool alreadyHasIdentityComponent = registryEntity.Has<TEntityIdentityComponent>();

            if (alreadyHasIdentityComponent)
            {
                ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();

                var previousEntity = getEntityFromIdentityComponentDelegate.Invoke(entityIdentityComponent);

                DespawnEntity(previousEntity);
            }

            //Get the target GUID from the registry entity
            var guid = registryEntity.Get<GUIDComponent>().GUID;

            if (!TrySpawnAndResolveEntityWithGUIDFromPrototype(
                prototypeID,
                guid,
                source,
                out localEntity))
            {
                registryEntity.Remove<TEntityIdentityComponent>();

                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TEntityIdentityComponent>(
                setIdentityComponentValuesDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        #endregion

        private bool TrySpawnEntity(
            string prototypeID,
            out Entity entity)
        {
            entity = default(Entity);

            if (string.IsNullOrEmpty(prototypeID))
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PROTOTYPE ID");

                return false;
            }

            if (!prototypeRepository.TryGetPrototype(
                prototypeID,
                out var prototypeEntity))
            {
                logger?.LogError(
                    GetType(),
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            entity = prototypeEntity.CopyTo(World);

            return true;
        }
    }
}