#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.HereticalEngine.Application;
using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public class Program
	{
		//TODO: https://github.com/dotnet/Silk.NET/discussions/534
		unsafe static void Main(string[] args)
		{
			//Create application context
			IApplicationContext context = ApplicationFactory.BuildApplicationContext();

			ICompositionRoot compositionRoot = context as ICompositionRoot;

			//var windowModule = new WindowModule(iocBuilder);

			//Load modules
			IModule[] modules = new IModule[]
			{
				//Bootstrapper modules, main container scope
				new ApplicationDataModule(),
				new LoggingModule(),
				new BuildDIContainerModule(),

				//Project scope
				new ApplicationLifetimeModule(),
				new SynchronizationModule(),
				new TimeModule(),
				new BuildLifetimeScopeModule(),

				//Scene scope
				new SceneLifetimeModule(),
				new BuildLifetimeScopeModule()
				
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
				compositionRoot.LoadModule(module);
			}

			//Lifetime

			//Start finishing the application by unloading the root lifetime module
			((ITearDownable)context.RootLifetime).TearDown();

			//Finish by unloading the bootstrap modules
			while (context.ActiveModules.Count() > 0)
			{
				Console.WriteLine($"Unloading module {context.ActiveModules.Last().Name}");

				compositionRoot.UnloadModule(context.ActiveModules.Last());
			}

			//End of application

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