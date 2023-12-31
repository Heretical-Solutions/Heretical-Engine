using HereticalSolutions.Logging;

using HereticalSolutions.Persistence;

using HereticalSolutions.Repositories;

using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    public class ECSEventWorldVisitor :
        ASaveLoadVisitor<World, ECSEventsBufferDTO>
    {
        private readonly IEventEntityBuilder<Entity> eventEntityBuilder;

        private readonly IReadOnlyRepository<int, Type> hashToType;
        
        private readonly IReadOnlyRepository<Type, int> typeToHash;
        
        private readonly VisitorReadComponentDelegate[] componentReaders;
        
        private readonly IReadOnlyRepository<Type, VisitorWriteComponentDelegate> componentWriters;
        
        private readonly List<ECSEventEntityDTO> entityDTOsCache;
        
        private readonly List<ECSComponentDTO> componentDTOsCache;

        private readonly bool host;

        private World cachedWorld;

        private EntitySet cachedEntitySet;
        

        public ECSEventWorldVisitor(
            IEventEntityBuilder<Entity> eventEntityBuilder,
            IReadOnlyRepository<int, Type> hashToType,
            IReadOnlyRepository<Type, int> typeToHash,
            VisitorReadComponentDelegate[] componentReaders,
            IReadOnlyRepository<Type, VisitorWriteComponentDelegate> componentWriters,
            bool host,
            IFormatLogger logger)
            : base (logger)
        {
            this.eventEntityBuilder = eventEntityBuilder;
            
            this.hashToType = hashToType;
            
            this.typeToHash = typeToHash;
            
            this.componentReaders = componentReaders;
            
            this.componentWriters = componentWriters;

            this.host = host;

            entityDTOsCache = new List<ECSEventEntityDTO>();
            
            componentDTOsCache = new List<ECSComponentDTO>();

            cachedWorld = null;

            cachedEntitySet = null;
        }

        #region ILoadVisitorGeneric

        public override bool Load(
            ECSEventsBufferDTO DTO,
            out World value)
        {
            value = new World();
            
            Load(DTO, value);
            
            return true;
        }

        public override bool Load(
            ECSEventsBufferDTO DTO,
            World valueToPopulate)
        {
            /*
            logger?.Log(
                GetType(),
                "PARSING EVENTS BUFFER");
            */
            
            ParseEventsBuffer(DTO);
            
            return true;
        }

        #endregion

        #region ISaveVisitorGeneric

        public override bool Save(
            World value, 
            out ECSEventsBufferDTO DTO)
        {
            if (value != cachedWorld)
            {
                cachedWorld = value;

                if (host)
                {
                    cachedEntitySet =
                        value
                            .GetEntities()
                            .With<NotifyPlayersComponent>()
                            .AsSet();
                }
                else
                {
                    cachedEntitySet =
                        value
                            .GetEntities()
                            .With<NotifyHostComponent>()
                            .AsSet();
                }
            }
            
            DTO = new ECSEventsBufferDTO();

            var eventEntities = cachedEntitySet;
            
            ParseEventEntities(
                eventEntities,
                ref DTO);
            
            return true;
        }

        #endregion

        private void ParseEventsBuffer(
            ECSEventsBufferDTO eventsBuffer)
        {
            for (int i = 0; i < eventsBuffer.EventEntities.Length; i++)
            {
                ParseEventEntity(eventsBuffer.EventEntities[i]);
            }
        }

        private void ParseEventEntity(
            ECSEventEntityDTO eventEntityDTO)
        {
            eventEntityBuilder.NewEvent(out var eventEntity);

            for (int i = 0; i < eventEntityDTO.Components.Length; i++)
            {
                var componentType = hashToType.Get(eventEntityDTO.Components[i].TypeHash);

                componentWriters.Get(componentType).Invoke(
                    eventEntity,
                    eventEntityDTO.Components[i]);
            }

            /*
            logger?.Log(
                GetType(),
                $"RECEIVED EVENT ENTITY FROM BUFFER, COMPONENTS AMOUNT: {eventEntityDTO.Components.Length}");
            */
        }

        private void ParseEventEntities(
            EntitySet eventEntities,
            ref ECSEventsBufferDTO DTO)
        {
            List<ECSEventEntityDTO> entityDTOs = entityDTOsCache;

            entityDTOsCache.Clear();

            foreach (ref readonly Entity eventEntity in eventEntities.GetEntities())
            {
                /*
                logger?.Log<ECSEventWorldVisitor>(
                    "PARSING EVENT ENTITY FROM EVENT WORLD");
                */

                ECSEventEntityDTO entityDTO = new ECSEventEntityDTO();

                List<ECSComponentDTO> componentDTOs = componentDTOsCache;

                componentDTOs.Clear();

                foreach (var typeSerializer in componentReaders)
                {
                    typeSerializer.Invoke(
                        eventEntity,
                        typeToHash,
                        componentDTOs);
                }

                entityDTO.Components = componentDTOs.ToArray();

                entityDTOs.Add(entityDTO);

                /*
                logger?.Log<ECSEventWorldVisitor>(
                    $"PARSED EVENT ENTITY, COMPONENT COUNT: {entityDTO.Components.Length}");
                */
            }

            foreach (ref readonly Entity eventEntity in eventEntities.GetEntities())
            {
                if (!host)
                {
                    eventEntity.Remove<NotifyHostComponent>();
                }
                else
                {
                    eventEntity.Remove<NotifyPlayersComponent>();
                }
            }

            DTO.EventEntities = entityDTOs.ToArray();
        }
    }
}