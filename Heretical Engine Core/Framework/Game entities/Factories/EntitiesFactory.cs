using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using DefaultEcs;

using HereticalSolutions.Logging;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.GameEntities.Factories
{
    /// <summary>
    /// Class containing methods to build entities and their components.
    /// </summary>
    public static class EntitiesFactory
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
        
        /// <summary>
        /// Builds an entity manager.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <returns>The built entity manager.</returns>
        public static EntityManager BuildEntityManager(
            ISmartLogger logger)
        {
            var entityRepository = RepositoriesFactory.BuildDictionaryRepository<Guid, Entity>();
        
            var prototypesRepository = RepositoriesFactory.BuildDictionaryRepository<string, Entity>();

            var worldsRepository = RepositoriesFactory.BuildDictionaryRepository<EWorld, World>();
            
            var worldControllersRepository = RepositoriesFactory.BuildDictionaryRepository<EWorld, IWorldController>();
            
            foreach(EWorld worldID in Enum.GetValues(typeof(EWorld)))
            { 
                worldsRepository.Add(
                    worldID,
                    new World());
            }

            var typeClosure = typeof(EntitiesFactory);
            
            worldControllersRepository.Add(
                EWorld.REGISTRY,
                new RegistryWorldController(
                    worldsRepository.Get(EWorld.REGISTRY),
                    logger));
            
            worldControllersRepository.Add(
                EWorld.SIMULATION,
                new WorldController<SimulationEntityComponent, ResolveSimulationComponent>(
                    worldsRepository.Get(EWorld.SIMULATION),
                    (component) => { return component.PrototypeID; },
                    (component, prototypeID) => 
                    { 
                        component.PrototypeID = prototypeID;
                        
                        return component; 
                    },
                    (component) => { return component.SimulationEntity; },
                    (component, entity) =>
                    {
                        component.SimulationEntity = entity;

                        return component;
                    },
                    (target) => { return new ResolveSimulationComponent { Target = target }; },
                    logger));
            
            worldControllersRepository.Add(
                EWorld.SERVER,
                new WorldController<ServerEntityComponent, ResolveServerComponent>(
                    worldsRepository.Get(EWorld.SERVER),
                    (component) => { return component.PrototypeID; },
                    (component, prototypeID) =>
                    {
                        component.PrototypeID = prototypeID;

                        return component;
                    },
                    (component) => { return component.ServerEntity; },
                    (component, entity) =>
                    {
                        component.ServerEntity = entity;

                        return component;
                    },
                    (target) => { return new ResolveServerComponent { Target = target }; },
                    logger));
            
            worldControllersRepository.Add(
                EWorld.PREDICTION,
                new WorldController<PredictionEntityComponent, ResolvePredictionComponent>(
                    worldsRepository.Get(EWorld.PREDICTION),
                    (component) => { return component.PrototypeID; },
                    (component, prototypeID) =>
                    {
                        component.PrototypeID = prototypeID;

                        return component;
                    },
                    (component) => { return component.PredictionEntity; },
                    (component, entity) =>
                    {
                        component.PredictionEntity = entity;

                        return component;
                    },
                    (target) => { return new ResolvePredictionComponent { Target = target }; },
                    logger));
            
            worldControllersRepository.Add(
                EWorld.VIEW,
                new WorldController<ViewEntityComponent, ResolveViewComponent>(
                    worldsRepository.Get(EWorld.VIEW),
                    (component) => { return component.PrototypeID; },
                    (component, prototypeID) =>
                    {
                        component.PrototypeID = prototypeID;

                        return component;
                    },
                    (component) => { return component.ViewEntity; },
                    (component, entity) =>
                    {
                        component.ViewEntity = entity;

                        return component;
                    },
                    (target) => { return new ResolveViewComponent { Target = target }; },
                    logger));

            EventEntityBuilder eventEntityBuilder = new EventEntityBuilder(
                worldsRepository.Get(
                    EWorld.EVENT));

            List<World> worlds = new List<World>();

            foreach (var worldID in worldsRepository.Keys)
            {
                worlds.Add(worldsRepository.Get(worldID));
            }

            MultiWorldSetter multiWorldSetter = new MultiWorldSetter(worlds.ToArray());
            
            return new EntityManager(
                entityRepository,
                prototypesRepository,
                worldsRepository,
                worldControllersRepository,
                eventEntityBuilder,
                multiWorldSetter,
                logger);
        }

        /// <summary>
        /// Builds an ECSWorldFullStateVisitor.
        /// </summary>
        /// <param name="entityManager">The entity manager to use.</param>
        /// <returns>The built ECSWorldFullStateVisitor.</returns>
        public static ECSWorldFullStateVisitor BuildECSWorldFullStateVisitor(
            IEntityManager entityManager,
            ISmartLogger logger)
        {
            BuildComponentTypesListWithAttribute<NetworkComponentAttribute>(
                out var componentTypes,
                out var hashToType,
                out var typeToHash);

            var readComponentMethodInfo =
                typeof(EntitiesFactory).GetMethod(
                    "ReadComponent",
                    BindingFlags.Static | BindingFlags.Public);
            
            var writeComponentMethodInfo =
                typeof(EntitiesFactory).GetMethod(
                    "WriteComponent",
                    BindingFlags.Static | BindingFlags.Public);
            
            return new ECSWorldFullStateVisitor(
                entityManager,
                hashToType,
                typeToHash,
                BuildComponentReaders(
                    readComponentMethodInfo,
                    componentTypes),
                BuildComponentWriters(
                    writeComponentMethodInfo,
                    componentTypes),
                logger);
        }
        
        public static ECSWorldMementoVisitor BuildECSWorldMementoVisitor(
            IEntityManager entityManager,
            ISmartLogger logger)
        {
            BuildComponentTypesListWithAttribute<NetworkComponentAttribute>(
                out var componentTypes,
                out var hashToType,
                out var typeToHash);

            var readComponentMethodInfo =
                typeof(EntitiesFactory).GetMethod(
                    "ReadComponent",
                    BindingFlags.Static | BindingFlags.Public);
            
            var writeComponentMethodInfo =
                typeof(EntitiesFactory).GetMethod(
                    "WriteComponent",
                    BindingFlags.Static | BindingFlags.Public);
            
            return new ECSWorldMementoVisitor(
                entityManager,
                hashToType,
                typeToHash,
                BuildComponentReaders(
                    readComponentMethodInfo,
                    componentTypes),
                BuildComponentWriters(
                    writeComponentMethodInfo,
                    componentTypes),
                new ECSWorldMemento(
                    RepositoriesFactory.BuildDictionaryRepository<Guid, ECSEntityMemento>(),
                    new List<ECSEntityCreatedDeltaDTO>(),
                    new List<ECSEntityDestroyedDeltaDTO>(),
                    new List<ECSComponentDeltaDTO>(),
                    new List<ECSComponentDeltaDTO>()),
                new HashSet<Guid>(),
                new List<ECSComponentDTO>(),
                logger);
        }

        public static ECSEventWorldVisitor BuildECSEventWorldVisitor(
            IEntityManager entityManager,
            bool host,
            ISmartLogger logger)
        {
            BuildComponentTypesListWithAttribute<NetworkEventComponentAttribute>(
                out var componentTypes,
                out var hashToType,
                out var typeToHash);

            var readComponentMethodInfo =
                typeof(EntitiesFactory).GetMethod(
                    "ReadComponent",
                    BindingFlags.Static | BindingFlags.Public);
            
            var writeComponentMethodInfo =
                typeof(EntitiesFactory).GetMethod(
                    "WriteComponent",
                    BindingFlags.Static | BindingFlags.Public);
            
            return new ECSEventWorldVisitor(
                entityManager,
                hashToType,
                typeToHash,
                BuildComponentReaders(
                    readComponentMethodInfo,
                    componentTypes),
                BuildComponentWriters(
                    writeComponentMethodInfo,
                    componentTypes),
                host,
                logger);
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
        
        private static VisitorReadComponentDelegate[] BuildComponentReaders(
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
        
        private static IReadOnlyRepository<Type, VisitorWriteComponentDelegate> BuildComponentWriters(
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
            //Debug.Log($"[ECSWorldFullStateVisitor] READING FROM COMPONENT {typeof(TComponent).ToString()}");
            
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
            //Debug.Log($"[ECSWorldFullStateVisitor] WRITING TO COMPONENT {typeof(TComponent).ToString()}");
            
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

        private static byte[] ToBytes(object component)
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

        private static object FromBytes(
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
        
        private static string ToDetailedString(object component)
        {
            var fields = component.GetType().GetFields();
            
            var sb = new StringBuilder();

            sb.Append("{");
            
            foreach (var fieldInfo in fields)
            {
                sb.Append($" {fieldInfo.Name} : {fieldInfo.GetValue(component)} |");
            }
            
            sb.Append("}");
            
            return sb.ToString();
        }
    }
}