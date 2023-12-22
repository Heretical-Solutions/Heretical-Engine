using DefaultEcs;

using HereticalSolutions.Logging;

using HereticalSolutions.Persistence;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.GameEntities
{
    public class ECSEventWorldVisitor :
        ILoadVisitorGeneric<World, ECSEventsBufferDTO>,
        ILoadVisitor,
        ISaveVisitorGeneric<World, ECSEventsBufferDTO>,
        ISaveVisitor
    {
        private readonly IEntityManager entityManager;
        
        private readonly IReadOnlyRepository<int, Type> hashToType;
        
        private readonly IReadOnlyRepository<Type, int> typeToHash;
        
        private readonly VisitorReadComponentDelegate[] componentReaders;
        
        private readonly IReadOnlyRepository<Type, VisitorWriteComponentDelegate> componentWriters;
        
        private readonly List<ECSEventEntityDTO> entityDTOsCache;
        
        private readonly List<ECSComponentDTO> componentDTOsCache;

        private readonly bool host;

        private readonly IFormatLogger logger;
        
        private World cachedWorld;

        private EntitySet cachedEntitySet;
        

        public ECSEventWorldVisitor(
            IEntityManager entityManager,
            IReadOnlyRepository<int, Type> hashToType,
            IReadOnlyRepository<Type, int> typeToHash,
            VisitorReadComponentDelegate[] componentReaders,
            IReadOnlyRepository<Type, VisitorWriteComponentDelegate> componentWriters,
            bool host,
            IFormatLogger logger)
        {
            this.entityManager = entityManager;
            
            this.hashToType = hashToType;
            
            this.typeToHash = typeToHash;
            
            this.componentReaders = componentReaders;
            
            this.componentWriters = componentWriters;

            this.host = host;

            this.logger = logger;

            entityDTOsCache = new List<ECSEventEntityDTO>();
            
            componentDTOsCache = new List<ECSComponentDTO>();

            cachedWorld = null;

            cachedEntitySet = null;
        }

        #region ILoadVisitorGeneric

        public bool Load(
            ECSEventsBufferDTO DTO,
            out World value)
        {
            value = new World();
            
            Load(DTO, value);
            
            return true;
        }

        public bool Load(
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
            entityManager.EventEntityBuilder.NewEvent(out var eventEntity);

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

        #endregion

        #region ILoadVisitor

        public bool Load<TValue>(
            object DTO, 
            out TValue value)
        {
            if (!(DTO.GetType().Equals(typeof(ECSEventsBufferDTO))))
                logger?.ThrowException<ECSEventWorldVisitor>(
                    $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(ECSEventsBufferDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");

            bool result = Load((ECSEventsBufferDTO)DTO, out World returnValue);
            
            value = result
                ? (TValue)(object)returnValue //DIRTY HACKS DO NOT REPEAT
                : default(TValue);
            
            return result;
        }

        public bool Load<TValue, TDTO>(
            TDTO DTO, 
            out TValue value)
        {
            if (!(typeof(TDTO).Equals(typeof(ECSEventsBufferDTO))))
                logger?.ThrowException<ECSEventWorldVisitor>(
                    $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(ECSEventsBufferDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            
            var dtoObject = (object)DTO;
            
            bool result = Load((ECSEventsBufferDTO)dtoObject, out World returnValue);
            
            value = result
                ? (TValue)(object)returnValue //DIRTY HACKS DO NOT REPEAT
                : default(TValue);
            
            return result;
        }

        public bool Load<TValue>(
            object DTO, 
            TValue valueToPopulate)
        {
            if (!(DTO.GetType().Equals(typeof(ECSEventsBufferDTO))))
                logger?.ThrowException<ECSEventWorldVisitor>(
                    $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(ECSEventsBufferDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            object objectValueToPopulate = (object)valueToPopulate;
            
            return Load(
                (ECSEventsBufferDTO)DTO,
                (World)objectValueToPopulate);
        }

        public bool Load<TValue, TDTO>(
            TDTO DTO, 
            TValue valueToPopulate)
        {
            if (!(typeof(TDTO).Equals(typeof(ECSEventsBufferDTO))))
                logger?.ThrowException<ECSEventWorldVisitor>(
                    $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(ECSEventsBufferDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;
 
            object valueToPopulateObject = (object)valueToPopulate;
            
            return Load(
                (ECSEventsBufferDTO)dtoObject,
                (World)valueToPopulateObject);
        }

        #endregion

        #region ISaveVisitorGeneric

        public bool Save(
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

        #endregion

        #region ISaveVisitor

        public bool Save<TValue>(
            TValue value, 
            out object DTO)
        {
            if (!(typeof(World).IsAssignableFrom(typeof(TValue))))
                logger?.ThrowException<ECSEventWorldVisitor>(
                    $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(World).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            object objectValue = (object)value;
            
            bool result = Save(
                (World)objectValue,
                out ECSEventsBufferDTO returnDTO);
            
            DTO = result
                ? returnDTO
                : default(object);
            
            return result;
        }

        public bool Save<TValue, TDTO>(
            TValue value, 
            out TDTO DTO)
        {
            if (!(typeof(World).IsAssignableFrom(typeof(TValue))))
                logger?.ThrowException<ECSEventWorldVisitor>(
                    $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(World).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            object objectValue = (object)value;
            
            bool result = Save(
                (World)objectValue,
                out ECSEventsBufferDTO returnDTO);

            if (result)
            {
                //DIRTY HACKS DO NOT REPEAT
                var dtoObject = (object)returnDTO;
                
                DTO = (TDTO)dtoObject;
                
            }
            else
            {
                DTO = default(TDTO);
            }

            return result;
        }

        #endregion
    }
}