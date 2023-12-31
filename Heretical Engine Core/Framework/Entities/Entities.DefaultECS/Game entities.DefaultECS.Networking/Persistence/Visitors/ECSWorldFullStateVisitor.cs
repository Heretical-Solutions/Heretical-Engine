using HereticalSolutions.Logging;

using HereticalSolutions.Persistence;

using HereticalSolutions.Repositories;

using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// A visitor class used to load and save the full state of an ECS world.
    /// </summary>
    public class ECSWorldFullStateVisitor :
        ASaveVisitor<World, ECSWorldDTO>
    {
        private readonly IEntityManager<World, Entity> entityManager;
        
        private readonly IReadOnlyRepository<int, Type> hashToType;
        
        private readonly IReadOnlyRepository<Type, int> typeToHash;
        
        private readonly VisitorReadComponentDelegate[] componentReaders;
        
        private readonly IReadOnlyRepository<Type, VisitorWriteComponentDelegate> componentWriters;
        
        private readonly List<ECSComponentDTO> componentDeltasCache;

        private World cachedWorld;

        private EntitySet cachedEntitySet;

        public ECSWorldFullStateVisitor(
            IEntityManager<World, Entity> entityManager,
            IReadOnlyRepository<int, Type> hashToType,
            IReadOnlyRepository<Type, int> typeToHash,
            VisitorReadComponentDelegate[] componentReaders,
            IReadOnlyRepository<Type, VisitorWriteComponentDelegate> componentWriters,
            IFormatLogger logger)
            : base (logger)
        {
            this.entityManager = entityManager;
            
            this.hashToType = hashToType;
            
            this.typeToHash = typeToHash;
            
            this.componentReaders = componentReaders;
            
            this.componentWriters = componentWriters;

            componentDeltasCache = new List<ECSComponentDTO>();

            cachedWorld = null;

            cachedEntitySet = null;
        }

        #region ISaveVisitorGeneric

        /// <summary>
        /// Saves the provided <see cref="World"/> value to an <see cref="ECSWorldDTO"/>.
        /// </summary>
        /// <param name="value">The <see cref="World"/> value to save.</param>
        /// <param name="DTO">The resulting <see cref="ECSWorldDTO"/>.</param>
        /// <returns><c>true</c> if the save operation was successful; otherwise, <c>false</c>.</returns>
        public override bool Save(
            World value, 
            out ECSWorldDTO DTO)
        {
            if (value != cachedWorld)
            {
                cachedWorld = value;
                
                cachedEntitySet = 
                    value
                        .GetEntities()
                        .AsSet();
            }

            DTO = new ECSWorldDTO();
            
            var entities = cachedEntitySet;
            
            DTO.EntityDTOs = new ECSEntityDTO[entities.Count];
            
            ParseEntities(
                entities,
                ref DTO);
            
            return true;
        }

        #endregion

        private void ParseEntities(
                    EntitySet entities,
                    ref ECSWorldDTO DTO)
        {
            int index = 0;

            foreach (ref readonly Entity entity in entities.GetEntities())
            {
                if (!entity.IsAlive) continue;

                var gameEntityComponent = entity.Get<GUIDComponent>();

                var guid = gameEntityComponent.GUID;

                var registryEntity = entityManager.GetRegistryEntity(guid);

                var registryEntityComponent = registryEntity.Get<RegistryEntityComponent>();


                DTO.EntityDTOs[index] = new ECSEntityDTO();

                DTO.EntityDTOs[index].GUID = guid.ToString();

                DTO.EntityDTOs[index].PrototypeID = registryEntityComponent.PrototypeID;


                List<ECSComponentDTO> componentDeltas = componentDeltasCache;

                componentDeltas.Clear();

                foreach (var typeSerializer in componentReaders)
                {
                    typeSerializer.Invoke(
                        entity,
                        typeToHash,
                        componentDeltas);
                }

                DTO.EntityDTOs[index].ComponentDTOs = componentDeltas.ToArray();

                index++;
            }
        }

        public void ReadComponent(
            Entity serverEntity,
            ECSComponentDTO componentDTO)
        {
            var componentType = hashToType.Get(componentDTO.TypeHash);

            componentWriters.Get(componentType).Invoke(
                serverEntity,
                componentDTO);
        }
    }
}