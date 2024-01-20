#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Modules;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public class Program
	{

		//TODO: https://github.com/dotnet/Silk.NET/discussions/534
		unsafe static void Main(string[] args)
		{
			IApplicationContext context = ApplicationFactory.BuildApplicationContext();

			ICompositionRoot compositionRoot = context as ICompositionRoot;

			//var windowModule = new WindowModule(iocBuilder);

			IModule[] modules = new IModule[]
			{
				//Core modules, main container scope
				new ApplicationDataModule(),
				new LoggingModule(),
				new DIContainerBuilderModule()

				//Project scope

				//Scene scope
				
				//windowModule,
				//new OpenGLModule(iocBuilder),
				//new MainCameraModule(iocBuilder),
				//new OpenGLDrawTestMeshModule(),

				//new TempModule()

				//new OpenGLDrawTestCubeModule(),
				//new OpenGLDrawTextureModule(),
				//new ImGuiModule()
			};

			foreach (var module in modules)
			{
				/*
				module.OnInitialized += () =>
				{
					logger?.Log<Program>(
						$"MODULE {module.GetType().Name} INITIALIZED");
				};

				module.OnCleanedUp += () =>
				{
					logger?.Log<Program>(
						$"MODULE {module.GetType().Name} CLEANED UP");
				};

				module.OnTornDown += () =>
				{
					logger?.Log<Program>(
						$"MODULE {module.GetType().Name} TORN DOWN");
				};
				*/

				module.Load(context);

				/*
				compositionRoot
					.ContainerBuilder
					.RegisterInstance(module)
					.Named<IModule>(
						module.Name);
				*/
			}

			/*

#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
			IRuntimeResourceManager runtimeResourceManager = ResourceManagementFactory.BuildConcurrentRuntimeResourceManager(logger);
#else
			IRuntimeResourceManager runtimeResourceManager = ResourceManagementFactory.BuildRuntimeResourceManager(logger);
#endif

			iocBuilder.RegisterInstance(runtimeResourceManager).As<IRuntimeResourceManager>();

			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer =
				new ConcurrentGenericCircularBuffer<MainThreadCommand>(
					new MainThreadCommand[1024],
					new int[1024]);

			iocBuilder.RegisterInstance(mainThreadCommandBuffer).As<ConcurrentGenericCircularBuffer<MainThreadCommand>>();

			ApplicationContext context = new ApplicationContext(
				modules,
				(ICoreModule)windowModule,
				runtimeResourceManager,
				
				mainThreadCommandBuffer,
				logger);

			//TODO: replace ApplicationContext dependencies whereever needed with direct dependencies
			iocBuilder.RegisterInstance(context).As<ApplicationContext>();

			for (int i = 0; i < context.Modules.Length; i++)
			{
				context.Modules[i].SetUp(context);
			}

			context.RootModule.Run(context);

			for (int i = 0; i < context.Modules.Length; i++)
			{
				context.Modules[i].TearDown();
			}

			if (logger != null)
			{
				((IDumpable)logger).Dump();
			}
			*/
		}
	}
}