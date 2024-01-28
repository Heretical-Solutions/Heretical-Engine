//#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.HereticalEngine.Application;
using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Synchronization;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public class Program
	{
		//TODO: https://github.com/dotnet/Silk.NET/discussions/534
		unsafe static void Main(string[] args)
		{
			//Start of application

			//Create application context
			IApplicationContext context = ApplicationFactory.BuildApplicationContext();

			IApplicationStatusManager applicationStatusManager = context as IApplicationStatusManager;

			//Application initialization

			//Load modules
			applicationStatusManager.SetStatus(EApplicationStatus.INITIALIZING);

			IModuleManager moduleManager = context as IModuleManager;

			IModule[] modules = new IModule[]
			{
				//Bootstrapper modules, main container scope
				new ApplicationDataModule(),
				new LoggingModule(),
				new BuildDIContainerModule(),

				//Application lifetime scope
				new ApplicationLifetimeModule(),
				new MainThreadCommandBufferModule(),
				new ResourceManagementModule(),
				new SynchronizationModule(),
				new TimeModule(),
				new RenderingModule(),
				new ApplicationSynchronizationPointsModule(),
				new BuildLifetimeScopeModule(),

				//Presentation lifetime scope
				new PresentationLifetimeModule(),
				new RenderingSynchronizationPointsModule(),
				new WindowSynchronizationPointsModule(),
				new RenderingTimeModule(),
				new SilkNETWindowModule(),
				new BuildLifetimeScopeModule(),

				//These two lifetimes (^ and v) should actually be branches of the application lifetime
				//TODO: the same presentation lifetime scope but for editor

				//Domain model lifetime scope
				new DomainModelLifetimeModule(),
				new BuildLifetimeScopeModule(),

				//Scene lifetime scope
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
				moduleManager.LoadModule(module);
			}

			applicationStatusManager.SetStatus(EApplicationStatus.INITIALIZED);

			//Lifetime
			applicationStatusManager.SetStatus(EApplicationStatus.RUNNING);

			//((ILifetimeScopeManager)context).CurrentLifetimeScope.TryResolve<IWindow>(out var window);
			//window?.Run();

			if (((ILifetimeScopeManager)context)
				.CurrentLifetimeScope
				.TryResolve<ISynchronizationManager>(
					out var synchronizationManager))
			{
				synchronizationManager.SynchronizeAll(ApplicationSynchronizationConstants.START_APPLICATION);
			}

			//Application shutdown
			applicationStatusManager.SetStatus(EApplicationStatus.SHUTTING_DOWN);

			//Start finishing the application by unloading the root lifetime module
			//TODO: move to the modules
			((ITearDownable)((ILifetimeComposer)context).RootLifetime).TearDown();

			//Finish by unloading the bootstrap modules
			while (context.ActiveModules.Count() > 0)
			{
				Console.WriteLine($"Unloading module {context.ActiveModules.Last().Name}");

				moduleManager.UnloadModule(context.ActiveModules.Last());
			}

			applicationStatusManager.SetStatus(EApplicationStatus.SHUTDOWN);

			//End of application
		}
	}
}