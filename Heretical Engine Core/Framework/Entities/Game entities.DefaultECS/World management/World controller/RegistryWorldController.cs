using DefaultEcs;
using DefaultEcs.System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.GameEntities
{
    public class RegistryWorldController
        : IWorldController<World, ISystem<Entity>, Entity>,
          IPrototypeCompliantWorldController<World, Entity>,
          IGUIDCompliantWorldController<Entity>
    {
        private readonly IPrototypesRepository<World, Entity> prototypeRepository;

        private readonly IFormatLogger logger;

        public RegistryWorldController(
            World world,
            IPrototypesRepository<World, Entity> prototypeRepository,
            IFormatLogger logger)
        {
            World = world;

            this.prototypeRepository = prototypeRepository;

            this.logger = logger;
        }
        
        #region IWorldController
        
        public World World { get; private set; }


        public ISystem<Entity> EntityResolveSystems { get => null; }

        public ISystem<Entity> EntityInitializationSystems { get => null; }

        public ISystem<Entity> EntityDeinitializationSystems { get => null; }


        public bool TrySpawnEntity(
            out Entity entity)
        {
            entity = World.CreateEntity();

            return true;
        }

        public bool TrySpawnAndResolveEntity(
            object source,
            out Entity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntity(
                out entity);
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

            entity.Set<DespawnComponent>();
        }

        #endregion

        #region IPrototypeCompliantWorldController

        public IPrototypesRepository<World, Entity> PrototypeRepository { get => prototypeRepository; }

        public bool TrySpawnEntityFromPrototype(
            string prototypeID,
            out Entity entity)
        {
            entity = default(Entity);

            if (string.IsNullOrEmpty(prototypeID))
            {
                logger?.LogError<RegistryWorldController>(
                    $"INVALID PROTOTYPE ID");

                return false;
            }

            if (!prototypeRepository.TryGetPrototype(
                prototypeID,
                out var prototypeEntity))
            {
                logger?.LogError<RegistryWorldController>(
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            entity = prototypeEntity.CopyTo(World);

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototype(
            string prototypeID,
            object source,
            out Entity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntityFromPrototype(
                prototypeID,
                out entity);
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

            ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();

            guidComponent.GUID = guid;

            return true;
        }

        #endregion
    }
}