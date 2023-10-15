using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    public class InitializeViewModelComponentSystem : ISystem<Entity>
    {
        private readonly IEntityManager entityManager;
        
        //private readonly bool isHost;
        
        public InitializeViewModelComponentSystem(
            IEntityManager entityManager) //,
            //bool isHost)
        {
            this.entityManager = entityManager;
            
            //this.isHost = isHost;
        }

        public bool IsEnabled { get; set; } = true;

        void ISystem<Entity>.Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<ViewModelComponent>())
                return;

            ref ViewModelComponent viewModelComponent = ref entity.Get<ViewModelComponent>();

            var guid = entity.Get<GameEntityComponent>().GUID;

            var registryEntity = entityManager.GetEntity(guid);

            if (registryEntity.Has<SimulationEntityComponent>())
            {
                var simulationEntity = registryEntity.Get<SimulationEntityComponent>().SimulationEntity;

                viewModelComponent.SourceEntity = simulationEntity;
            }

            if (registryEntity.Has<PredictionEntityComponent>())
            {
                var predictionEntity = registryEntity.Get<PredictionEntityComponent>().PredictionEntity;

                viewModelComponent.SourceEntity = predictionEntity;
            }
            
            /*
            if (isHost)
            {
                if (!registryEntity.Has<SimulationEntityComponent>())
                    return;
                
                var simulationEntity = registryEntity.Get<SimulationEntityComponent>().SimulationEntity;

                viewModelComponent.SourceEntity = simulationEntity;
            }
            else
            {
                {
                    if (!registryEntity.Has<PredictionEntityComponent>())
                        return;
                
                    var predictionEntity = registryEntity.Get<PredictionEntityComponent>().PredictionEntity;

                    viewModelComponent.SourceEntity = predictionEntity;
                }
            }
            */
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
        }
    }
}