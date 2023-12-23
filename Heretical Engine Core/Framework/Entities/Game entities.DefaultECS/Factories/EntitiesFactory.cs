using System.Reflection;
using System.Runtime.InteropServices;

using HereticalSolutions.Logging;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using DefaultEcs;

using IEntityWorldsRepository =
    HereticalSolutions
    .GameEntities
    .IEntityWorldsRepository<
        DefaultEcs.World,
        DefaultEcs.System.ISystem<DefaultEcs.Entity>,
        DefaultEcs.Entity>;

using IWorldController =
    HereticalSolutions
    .GameEntities
    .IWorldController<
        DefaultEcs.World,
        DefaultEcs.System.ISystem<DefaultEcs.Entity>,
        DefaultEcs.Entity>;

namespace HereticalSolutions.GameEntities.Factories
{
    /// <summary>
    /// Class containing methods to build entities and their components.
    /// </summary>
    public static partial class EntitiesFactory
    {
        /// <summary>
        /// Delegate for reading components of an entity.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to read.</typeparam>
        /// <param name="entity">The entity to read the component from.</param>
        /// <param name="typeToHash">The repository mapping component types to their hash values.</param>
        /// <param name="componentDeltas">A list of component deltas for the entity.</param>
        public delegate void ComponentReaderDelegate<TComponent>(
            Entity entity,
            IReadOnlyRepository<Type, int> typeToHash,
            List<ECSComponentDTO> componentDTOs);


        public static EntityManager BuildSimpleEntityManager(
            IFormatLogger logger)
        {
            var registryEntityRepository = RepositoriesFactory.BuildDictionaryRepository<Guid, Entity>();

            var entityWorldsRepository = BuildEntityWorldsRepository(logger);


            entityWorldsRepository.AddWorld(
                WorldConstants.REGISTRY_WORLD_ID,
                BuildRegistryWorldController(
                    BuildPrototypesRepository(),
                    logger));

            entityWorldsRepository.AddWorld(
                WorldConstants.EVENT_WORLD_ID,
                BuildEventWorldController(
                    logger));

            entityWorldsRepository.AddWorld(
                WorldConstants.SIMULATION_WORLD_ID,
                BuildWorldController<SimulationEntityComponent, ResolveSimulationComponent>(
                    (component) => { return component.SimulationEntity; },
                    (component) => { return component.PrototypeID; },
                    (prototypeID, entity) => 
                    {
                        return new SimulationEntityComponent
                        {
                            PrototypeID = prototypeID,

                            SimulationEntity = entity
                        };
                    },
                    (source) => { return new ResolveSimulationComponent { Source = source }; },
                    logger));

            entityWorldsRepository.AddWorld(
                WorldConstants.VIEW_WORLD_ID,
                BuildWorldController<ViewEntityComponent, ResolveViewComponent>(
                    (component) => { return component.ViewEntity; },
                    (component) => { return component.PrototypeID; },
                    (prototypeID, entity) =>
                    {
                        return new ViewEntityComponent
                        {
                            PrototypeID = prototypeID,

                            ViewEntity = entity
                        };
                    },
                    (source) => { return new ResolveViewComponent { Source = source }; },
                    logger));

            List<World> childEntityWorlds = new List<World>();

            childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID));
            childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.VIEW_WORLD_ID));

            return new EntityManager(
                registryEntityRepository,
                entityWorldsRepository,
                childEntityWorlds,
                logger);
        }

        public static EventWorldController BuildEventWorldController(
            IFormatLogger logger)
        {
            World world = new World();

            return new EventWorldController(
                world,
                logger);
        }

        public static RegistryWorldController BuildRegistryWorldController(
            IPrototypesRepository<World, Entity> prototypeRepository,
            IFormatLogger logger)
        {
            World world = new World();

            return new RegistryWorldController(
                world,
                prototypeRepository,
                logger);
        }

        public static WorldController<TEntityIdentityComponent, TResolveComponent>
            BuildWorldController<TEntityIdentityComponent, TResolveComponent>(
                Func<TEntityIdentityComponent, Entity> getEntityFromIdentityComponentDelegate,
                Func<TEntityIdentityComponent, string> getPrototypeIDFromIdentityComponentDelegate,
                Func<string, Entity, TEntityIdentityComponent> setIdentityComponentValuesDelegate,
                Func<object, TResolveComponent> createResolveComponentDelegate,
                IFormatLogger logger)
        {
            World world = new World();

            return new WorldController<TEntityIdentityComponent, TResolveComponent>(
                world,
                getEntityFromIdentityComponentDelegate,
                getPrototypeIDFromIdentityComponentDelegate,
                setIdentityComponentValuesDelegate,
                createResolveComponentDelegate,
                BuildPrototypesRepository(),
                logger);
        }

        public static PrototypesRepository BuildPrototypesRepository()
        {
            return new PrototypesRepository(
                new World(),
                RepositoriesFactory.BuildDictionaryRepository<string, Entity>());
        }

        public static IEntityWorldsRepository BuildEntityWorldsRepository(
            IFormatLogger logger)
        {
            var worldsRepository = RepositoriesFactory.BuildDictionaryRepository<string, World>();

            var worldControllersRepository = RepositoriesFactory.BuildDictionaryRepository<World, IWorldController>();

            return new EntityWorldsRepository(
                worldsRepository,
                worldControllersRepository,
                logger);
        }

        public static EventEntityBuilder BuildEventEntityBuilder(
            World world)
        {
            return new EventEntityBuilder(world);
        }

        /// <summary>
        /// Builds a list of component types with the specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to filter the component types with.</typeparam>
        /// <param name="componentTypes">The resulting list of component types.</param>
        public static void BuildComponentTypesListWithAttribute<TAttribute>(
            out Type[] componentTypes)
            where TAttribute : System.Attribute
        {
            List<Type> result = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<TAttribute>(false) != null)
                    {
                        result.Add(type);
                    }
                }
            }

            componentTypes = result.ToArray();
        }
        
        /// <summary>
        /// Builds a list of component types with the specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to filter the component types with.</typeparam>
        /// <param name="componentTypes">The resulting list of component types.</param>
        /// <param name="hashToType">The repository mapping component hash values to their types.</param>
        /// <param name="typeToHash">The repository mapping component types to their hash values.</param>
        public static void BuildComponentTypesListWithAttribute<TAttribute>(
            out Type[] componentTypes,
            out IReadOnlyRepository<int, Type> hashToType,
            out IReadOnlyRepository<Type, int> typeToHash)
            where TAttribute : System.Attribute
        {
            hashToType = RepositoriesFactory.BuildDictionaryRepository<int, Type>();

            typeToHash = RepositoriesFactory.BuildDictionaryRepository<Type, int>();

            List<Type> result = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<TAttribute>(false) != null)
                    {
                        result.Add(type);
                    }
                }
            }

            foreach (Type type in result)
            {
                string typeFullString = type.ToString();

                int typeHash = typeFullString.GetHashCode();
                
                ((IRepository<int, Type>)hashToType).AddOrUpdate(typeHash, type);
                
                ((IRepository<Type, int>)typeToHash).AddOrUpdate(type, typeHash);
            }

            componentTypes = result.ToArray();
        }
        
        public static VisitorReadComponentDelegate[] BuildComponentReaders(
            MethodInfo readComponentMethodInfo,
            Type[] componentTypes)
        {
            var result = new VisitorReadComponentDelegate[componentTypes.Length];

            for (int i = 0; i < result.Length; i++)
            {
                MethodInfo readComponentGeneric = readComponentMethodInfo.MakeGenericMethod(componentTypes[i]);
                
                VisitorReadComponentDelegate readComponentGenericDelegate =
                    (VisitorReadComponentDelegate)readComponentGeneric.CreateDelegate(
                        typeof(VisitorReadComponentDelegate),
                        null);

                result[i] = readComponentGenericDelegate;
            }

            return result;
        }
        
        public static IReadOnlyRepository<Type, VisitorWriteComponentDelegate> BuildComponentWriters(
            MethodInfo writeComponentMethodInfo,
            Type[] componentTypes)
        {
            IReadOnlyRepository<Type, VisitorWriteComponentDelegate> result = RepositoriesFactory.BuildDictionaryRepository<Type, VisitorWriteComponentDelegate>();

            for (int i = 0; i < componentTypes.Length; i++)
            {
                MethodInfo writeComponentGeneric = writeComponentMethodInfo.MakeGenericMethod(componentTypes[i]);
                
                VisitorWriteComponentDelegate writeComponentGenericDelegate =
                    (VisitorWriteComponentDelegate)writeComponentGeneric.CreateDelegate(
                        typeof(VisitorWriteComponentDelegate),
                        null);

                ((IRepository<Type, VisitorWriteComponentDelegate>)result).Add(
                    componentTypes[i],
                    writeComponentGenericDelegate);
            }

            return result;
        }
        
        /// <summary>
        /// Reads a component of type <typeparamref name="TComponent"/> from the provided entity and adds it to the component deltas list.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to read.</typeparam>
        /// <param name="entity">The entity to read the component from.</param>
        /// <param name="typeToHash">The repository that maps types to type hash codes.</param>
        /// <param name="componentDTOs">The list to store the component DTOs.</param>
        public static void ReadComponent<TComponent>(
            Entity entity,
            IReadOnlyRepository<Type, int> typeToHash,
            List<ECSComponentDTO> componentDTOs)
        {
            //Early return for AoT compilation calls
            if (componentDTOs == null)
                return;
            
            if (!entity.Has<TComponent>())
            {
                return;
            }

            var dto = new ECSComponentDTO();
            
            dto.TypeHash = typeToHash.Get(typeof(TComponent));
            
            dto.Data = ToBytes(entity.Get<TComponent>());
            
            componentDTOs.Add(dto);
        }
        
        /// <summary>
        /// Writes a component of type <typeparamref name="TComponent"/> to the provided entity using the component delta data.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to write.</typeparam>
        /// <param name="entity">The entity to write the component to.</param>
        /// <param name="componentDTO">The component delta data.</param>
        public static void WriteComponent<TComponent>(
            Entity entity,
            ECSComponentDTO componentDTO)
        {
            //Early return for AoT compilation calls
            if (componentDTO.Data == null)
                return;
            
            /*
            if (!entity.Has<TComponent>())
            {
                return;
            }
            */

            var component = (TComponent)FromBytes(componentDTO.Data, typeof(TComponent));
            
            entity.Set<TComponent>(component);
        }

        public static byte[] ToBytes(object component)
        {
            int componentSize = Marshal.SizeOf(component);
            
            byte[] result = new byte[componentSize];
            
            IntPtr ptr = IntPtr.Zero;
            
            try
            {
                ptr = Marshal.AllocHGlobal(componentSize);
                
                Marshal.StructureToPtr(
                    component,
                    ptr,
                    true);
                
                Marshal.Copy(
                    ptr,
                    result,
                    0,
                    componentSize);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            
            return result;
        }

        public static object FromBytes(
            byte[] data,
            Type componentType)
        {
            object result;
            
            int size = Marshal.SizeOf(componentType);
            
            IntPtr ptr = IntPtr.Zero;
            
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                
                Marshal.Copy(
                    data,
                    0,
                    ptr,
                    size);
                
                result = Marshal.PtrToStructure(
                    ptr,
                    componentType);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            
            return result;
        }
    }
}