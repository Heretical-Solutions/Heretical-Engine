using HereticalSolutions.Repositories;

namespace HereticalSolutions.GameEntities
{
    public class ECSWorldMemento
    {
        public IRepository<Guid, ECSEntityMemento> EntityMementos { get; private set; }

        public List<ECSEntityCreatedDeltaDTO> EntitiesCreated { get; private set; }

        public List<ECSEntityDestroyedDeltaDTO> EntitiesDestroyed { get; private set; }

        public List<ECSComponentDeltaDTO> ComponentsCreated { get; private set; }
        
        public List<ECSComponentDeltaDTO> ComponentsModified { get; private set; }

        public ECSWorldMemento(
            IRepository<Guid, ECSEntityMemento> entityMementos,
            List<ECSEntityCreatedDeltaDTO> entitiesCreated,
            List<ECSEntityDestroyedDeltaDTO> entitiesDestroyed,
            List<ECSComponentDeltaDTO> componentsCreated,
            List<ECSComponentDeltaDTO> componentsModified)
        {
            EntityMementos = entityMementos;

            EntitiesCreated = entitiesCreated;

            EntitiesDestroyed = entitiesDestroyed;

            ComponentsCreated = componentsCreated;
            
            ComponentsModified = componentsModified;
        }

        public ECSWorldDTO Dump()
        {
            ECSWorldDTO dto = new ECSWorldDTO();

            dto.EntityDTOs = new ECSEntityDTO[EntityMementos.Keys.Count()];

            int entityIndex = 0;

            foreach (var guid in EntityMementos.Keys)
            {
                var entityMemento = EntityMementos.Get(guid);
                
                dto.EntityDTOs[entityIndex] = new ECSEntityDTO();

                dto.EntityDTOs[entityIndex].GUID = entityMemento.GUID;

                dto.EntityDTOs[entityIndex].PrototypeID = entityMemento.PrototypeID;

                dto.EntityDTOs[entityIndex].ComponentDTOs = new ECSComponentDTO[entityMemento.ComponentDTOs.Keys.Count()];

                int componentIndex = 0;
                
                foreach (var typeHash in entityMemento.ComponentDTOs.Keys)
                {
                    dto.EntityDTOs[entityIndex].ComponentDTOs[componentIndex] =
                        entityMemento.ComponentDTOs.Get(typeHash); 
                    
                    componentIndex++;
                }

                entityIndex++;
            }

            return dto;
        }
    }
}