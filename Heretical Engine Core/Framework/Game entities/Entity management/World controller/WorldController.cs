using System;

using DefaultEcs;
using DefaultEcs.System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.GameEntities
{
    public class WorldController<TEntityIdentityComponent, TResolveComponent>
        : IWorldController,
          IWorldControllerInternal
    {
        private readonly Func<TEntityIdentityComponent, string> getPrototypeIDDelegate;
        
        private readonly Func<TEntityIdentityComponent, string, TEntityIdentityComponent> setPrototypeIDDelegate;
        
        private readonly Func<TEntityIdentityComponent, Entity> getEntityFromIdentityComponentDelegate;
        
        private readonly Func<TEntityIdentityComponent, Entity, TEntityIdentityComponent> setEntityToIdentityComponentDelegate;
        
        private readonly Func<object, TResolveComponent> createResolveComponentDelegate;
        
        private readonly IFormatLogger logger;
        

        private IEntityManagerInternal entityManagerInternal;
        
        private ISystem<Entity> resolveSystems;
        
        private ISystem<Entity> initializationSystems;
        
        private ISystem<Entity> deinitializationSystems;

        public WorldController(
            World world,
            Func<TEntityIdentityComponent, string> getPrototypeIDDelegate,
            Func<TEntityIdentityComponent, string, TEntityIdentityComponent> setPrototypeIDDelegate,
            Func<TEntityIdentityComponent, Entity> getEntityFromIdentityComponentDelegate,
            Func<TEntityIdentityComponent, Entity, TEntityIdentityComponent> setEntityToIdentityComponentDelegate,
            Func<object, TResolveComponent> createResolveComponentDelegate,
            IFormatLogger logger)
        {
            World = world;

            this.getPrototypeIDDelegate = getPrototypeIDDelegate;

            this.setPrototypeIDDelegate = setPrototypeIDDelegate;

            this.getEntityFromIdentityComponentDelegate = getEntityFromIdentityComponentDelegate;

            this.setEntityToIdentityComponentDelegate = setEntityToIdentityComponentDelegate;

            this.createResolveComponentDelegate = createResolveComponentDelegate;

            this.logger = logger;
        }
        
        #region IWorldController
        
        public World World { get; private set; }
        
        public void Initialize(
            ISystem<Entity> resolveSystems,
            ISystem<Entity> initializationSystems,
            ISystem<Entity> deinitializationSystems)
        {
            this.resolveSystems = resolveSystems;

            this.initializationSystems = initializationSystems;

            this.deinitializationSystems = deinitializationSystems;
        }
        
        public bool AddEntity(
            Entity registryEntity,
            string prototypeID)
        {
            if (registryEntity.Has<TEntityIdentityComponent>())
            {
                return false;
            }

            registryEntity.Set<TEntityIdentityComponent>();

            ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();

            //entityIdentityComponent.PrototypeID = prototypeID;
            registryEntity.Set<TEntityIdentityComponent>(
                setPrototypeIDDelegate.Invoke(
                    entityIdentityComponent,
                    prototypeID));
            
            SpawnEntity(registryEntity);
            
            return true;
        }
        
        public void DespawnEntity(
            Entity registryEntity)
        {
            if (!registryEntity.Has<TEntityIdentityComponent>())
                return;

            ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();

            //var entity = entityIdentityComponent.ViewEntity;
            var entity = getEntityFromIdentityComponentDelegate.Invoke(entityIdentityComponent);
            
            entity.Set<DespawnComponent>();
            
            registryEntity.Remove<TEntityIdentityComponent>();
            
            deinitializationSystems?.Update(entity);
        }
        
        public void ReplaceEntity(
            Entity registryEntity,
            string prototypeID)
        {
            bool alreadyHasIdentityEntity = registryEntity.Has<TEntityIdentityComponent>(); 
            
            if (!alreadyHasIdentityEntity)
            {
                registryEntity.Set<TEntityIdentityComponent>();
            }
            
            ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();

            if (alreadyHasIdentityEntity)
            {
                //var previousViewEntity = entityIdentityComponent.ViewEntity;
                var previousEntity = getEntityFromIdentityComponentDelegate.Invoke(entityIdentityComponent);
            
                previousEntity.Set<DespawnComponent>();
                
                deinitializationSystems?.Update(previousEntity);
            }
            
            //entityIdentityComponent.PrototypeID = prototypeID;
            registryEntity.Set<TEntityIdentityComponent>(
                setPrototypeIDDelegate.Invoke(
                    entityIdentityComponent,
                    prototypeID));
            
            SpawnEntity(registryEntity);
        }
        
        #endregion

        #region IWorldControllerInternal

        public void InitializeEntityManagerInternal(
            IEntityManagerInternal entityManagerInternal)
        {
            this.entityManagerInternal = entityManagerInternal;
        }
        
        public void SpawnEntityFromPrototype(
            Entity registryEntity)
        {
            if (!registryEntity.Has<TEntityIdentityComponent>())
                return;

            SpawnEntity(registryEntity);
        }
        
        public void SpawnAndResolveEntityFromPrototype(
            Entity registryEntity,
            object target)
        {
            if (!registryEntity.Has<TEntityIdentityComponent>())
                return;

            SpawnAndResolveEntity(
                registryEntity,
                target);
        }
        
        public bool SpawnEntity(
            string prototypeID,
            Guid guid,
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

            /*
            if (!entityManagerInternal.WorldsRepository.TryGet(EWorld.PROTOTYPE, out var prototypesWorld))
            {
                logger?.LogError(
                    GetType(),
                    "NO PROTOTYPES WORLD REGISTERED");
                
                return false;
            }
            */
            
            if (!entityManagerInternal.PrototypesRepository.TryGet(prototypeID, out var prototypeEntity))
            {
                logger?.LogError(
                    GetType(),
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            entity = prototypeEntity.CopyTo(World);

            ref GameEntityComponent gameEntityComponent = ref entity.Get<GameEntityComponent>();

            gameEntityComponent.GUID = guid;

            return true;
        }

        #endregion

        private void SpawnEntity(
            Entity registryEntity)
        {
            var guid = registryEntity.Get<GameEntityComponent>().GUID;

            ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();

            var prototypeID = getPrototypeIDDelegate.Invoke(entityIdentityComponent);
            
            
            if (!SpawnEntity(
                    prototypeID,
                    guid,
                    out var entity))
            {
                registryEntity.Remove<TEntityIdentityComponent>();
                
                return;
            }
            
            ref GameEntityComponent gameEntityComponent = ref entity.Get<GameEntityComponent>();

            gameEntityComponent.GUID = guid;

            //entityIdentityComponent.ViewEntity = entity;
            registryEntity.Set<TEntityIdentityComponent>(
                setEntityToIdentityComponentDelegate.Invoke(
                    entityIdentityComponent,
                    entity));

            initializationSystems?.Update(entity);
        }
        
        private void SpawnAndResolveEntity(
            Entity registryEntity,
            object target)
        {
            var guid = registryEntity.Get<GameEntityComponent>().GUID;

            ref var entityIdentityComponent = ref registryEntity.Get<TEntityIdentityComponent>();
            
            var prototypeID = getPrototypeIDDelegate.Invoke(entityIdentityComponent);
            
            
            if (!SpawnEntity(
                    prototypeID,
                    guid,
                    out var entity))
            {
                registryEntity.Remove<TEntityIdentityComponent>();
                
                return;
            }
            
            
            ref GameEntityComponent gameEntityComponent = ref entity.Get<GameEntityComponent>();

            gameEntityComponent.GUID = guid;

            //entityIdentityComponent.ViewEntity = viewEntity;
            registryEntity.Set<TEntityIdentityComponent>(
                setEntityToIdentityComponentDelegate.Invoke(
                    entityIdentityComponent,
                    entity));
            
            
            /*
            entity.Set<ResolveViewComponent>(
                new ResolveViewComponent
                {
                    Target = target
                });
            */
            entity.Set<TResolveComponent>(
                createResolveComponentDelegate.Invoke(target));
            
            resolveSystems?.Update(entity);
            
            initializationSystems?.Update(entity);
        }
    }
}