//#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.HereticalEngine.Application;
using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Hierarchy;

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


			//Root lifetime
			var rootLifetimeModule = new RootLifetimeModule(
				new List<IReadOnlyHierarchyNode>(),
				new List<Action<ContainerBuilder>>(),
				new IModule[]
				{
					new ApplicationContextModule(),
					new ApplicationDataModule(),
					new LoggingModule()
				});

			moduleManager.LoadModule(
				rootLifetimeModule,
				null);

			var rootLifetime = (ILifetimeModule)rootLifetimeModule;


			//Application lifetime
			var ApplicationLifetimeModule = new ApplicationLifetimeModule(
				new List<IReadOnlyHierarchyNode>(),
				new List<Action<ContainerBuilder>>(),
				new IModule[]
				{
					new MainThreadCommandBufferModule(),
					new ResourceManagementModule(),
					new SynchronizationModule(),
					new TimeModule(),
					new RenderingModule(),
					new ApplicationSynchronizationPointsModule()
				});

			moduleManager.LoadModule(
				ApplicationLifetimeModule,
				rootLifetime);

			var applicationLifetime = (ILifetimeModule)ApplicationLifetimeModule;


			//Presentation lifetime
			var presentationLifetimeModule = new PresentationLifetimeModule(
				new List<IReadOnlyHierarchyNode>(),
				new List<Action<ContainerBuilder>>(),
				new IModule[]
				{
					new RenderingSynchronizationPointsModule(),
					new WindowSynchronizationPointsModule(),
					new RenderingTimeModule(),
					new SilkNETWindowModule()
				});

			moduleManager.LoadModule(
				presentationLifetimeModule,
				applicationLifetime);

			var presentationLifetime = (ILifetimeModule)presentationLifetimeModule;


			applicationStatusManager.SetStatus(EApplicationStatus.INITIALIZED);

			//Update loop
			applicationStatusManager.SetStatus(EApplicationStatus.RUNNING);

			//((ILifetimeScopeManager)context).CurrentLifetimeScope.TryResolve<IWindow>(out var window);
			//window?.Run();

			if (applicationLifetime
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
			((ITearDownable)context.RootLifetime).TearDown();

			applicationStatusManager.SetStatus(EApplicationStatus.SHUTDOWN);

			//End of application
		}
	}
}