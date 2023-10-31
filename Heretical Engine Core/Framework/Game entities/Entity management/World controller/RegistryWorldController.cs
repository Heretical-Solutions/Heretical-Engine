using System;

using DefaultEcs;
using DefaultEcs.System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.GameEntities
{
    public class RegistryWorldController
        : IWorldController,
          IWorldControllerInternal
    {
        private readonly IFormatLogger logger;

        private IEntityManagerInternal entityManagerInternal;
        
        public RegistryWorldController(
            World world,
            IFormatLogger logger)
        {
            World = world;

            this.logger = logger;
        }
        
        #region IWorldController
        
        public World World { get; private set; }

        public void Initialize(
            ISystem<Entity> resolveSystems,
            ISystem<Entity> initializationSystems,
            ISystem<Entity> deinitializationSystems)
        {
            logger.ThrowException<RegistryWorldController>(
                "REGISTRY WORLD SHOULD NOT CONTAIN INITIALIZATION, RESOLVE AND DEINITIALIZATION SYSTEMS");
        }

        public bool AddEntity(
            Entity registryEntity,
            string prototypeID)
        {
            logger.ThrowException<RegistryWorldController>(
                "REGISTRY WORLD SHOULD NOT BE ABLE TO ADD REGISTRY ENTITY TO ANOTHER REGISTRY ENTITY");

            return false;
        }
        
        public void DespawnEntity(
            Entity registryEntity)
        {
            logger.ThrowException<RegistryWorldController>(
                "REGISTRY WORLD SHOULD NOT BE ABLE TO DELETE REGISTRY ENTITY FROM ANOTHER REGISTRY ENTITY");   
        }
        
        public void ReplaceEntity(
            Entity registryEntity,
            string prototypeID)
        {
            logger.ThrowException<RegistryWorldController>(
                "REGISTRY WORLD SHOULD NOT BE ABLE TO REPLACE REGISTRY ENTITY AT ANOTHER REGISTRY ENTITY");
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
            logger.ThrowException<RegistryWorldController>(
                "REGISTRY ENTITY SPAWNING IS HANDLED BY ENTITY MANAGER");
        }
        
        public void SpawnAndResolveEntityFromPrototype(
            Entity registryEntity,
            object target)
        {
            logger.ThrowException<RegistryWorldController>(
                "REGISTRY ENTITY RESOLVING IS HANDLED BY ENTITY MANAGER");
        }
        
        public bool SpawnEntity(
            string prototypeID,
            Guid guid,
            out Entity entity)
        {
            entity = default(Entity);
            
            if (string.IsNullOrEmpty(prototypeID))
            {
                logger.LogError<RegistryWorldController>(
                    $"INVALID PROTOTYPE ID");
                
                return false;
            }

            if (!entityManagerInternal.WorldsRepository.TryGet(EWorld.PROTOTYPE, out var prototypesWorld))
            {
                logger.LogError<RegistryWorldController>(
                    "NO PROTOTYPES WORLD REGISTERED");
                
                return false;
            }
            
            if (!entityManagerInternal.PrototypesRepository.TryGet(prototypeID, out var prototypeEntity))
            {
                logger.LogError<RegistryWorldController>(
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            entity = prototypeEntity.CopyTo(World);

            ref GameEntityComponent gameEntityComponent = ref entity.Get<GameEntityComponent>();

            gameEntityComponent.GUID = guid;

            return true;
        }

        #endregion
    }
}