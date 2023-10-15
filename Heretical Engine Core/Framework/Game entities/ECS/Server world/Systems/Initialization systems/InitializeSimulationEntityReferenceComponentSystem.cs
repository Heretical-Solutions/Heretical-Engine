using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    public class InitializeSimulationEntityReferenceComponentSystem : ISystem<Entity>
    {
        private readonly IEntityManager entityManager;

        private readonly bool isHost;
        
        public InitializeSimulationEntityReferenceComponentSystem(
            IEntityManager entityManager,
            bool isHost)
        {
            this.entityManager = entityManager;

            this.isHost = isHost;
        }

        public bool IsEnabled { get; set; } = true;

        void ISystem<Entity>.Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<SimulationEntityReferenceComponent>())
                return;

            if (!isHost)
            {
                entity.Remove<SimulationEntityReferenceComponent>();
                
                return;
            }

            ref SimulationEntityReferenceComponent simulationEntityReferenceComponent = ref entity.Get<SimulationEntityReferenceComponent>();

            var guid = entity.Get<GameEntityComponent>().GUID;

            var registryEntity = entityManager.GetEntity(guid);

            if (!registryEntity.Has<SimulationEntityComponent>())
                return;
            
            var simulationEntity = registryEntity.Get<SimulationEntityComponent>().SimulationEntity;

            simulationEntityReferenceComponent.SimulationEntity = simulationEntity;
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
        }
    }
}