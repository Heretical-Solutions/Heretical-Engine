using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ResourceManagementModule
		: ALifetimeableModule
	{
		public override string Name => "Resource management module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;

			var containerBuilder = compositionRoot.ContainerBuilder;

			containerBuilder
				.Register(componentContext =>
				{
					componentContext.TryResolve<ILoggerResolver>(
						out ILoggerResolver loggerResolver);

					var logger = loggerResolver?.GetLogger<ResourceManagementModule>();

					logger?.Log<ResourceManagementModule>(
						"BUILDING RUNTIME RESOURCE MANAGER");

#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
			IRuntimeResourceManager runtimeResourceManager = 
				ResourceManagementFactory.BuildConcurrentRuntimeResourceManager(loggerResolver);
#else
			IRuntimeResourceManager runtimeResourceManager = 
				ResourceManagementFactory.BuildRuntimeResourceManager(loggerResolver);
#endif

					return runtimeResourceManager;
				})
				.As<IRuntimeResourceManager>()
				.SingleInstance();

			base.InitializeInternal();
		}

		protected override void CleanupInternal()
		{
			if (context
				.CurrentLifetimeScope
				.TryResolve<IRuntimeResourceManager>(
					out IRuntimeResourceManager runtimeResourceManager))
			{
				runtimeResourceManager.ClearAllRootResources();
			}

			base.CleanupInternal();
		}
	}
}