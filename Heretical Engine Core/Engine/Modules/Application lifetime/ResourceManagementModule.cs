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
			var lifetimeScopeManager = parentLifetime as ILifetimeScopeContainer;

			lifetimeScopeManager.QueueLifetimeScopeAction(
				containerBuilder =>
				{
					containerBuilder
						.Register(componentContext =>
						{
							componentContext.TryResolve<ILoggerResolver>(
								out ILoggerResolver loggerResolver);
							
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

					//For some fucking reason autofac performs delegates in lifetime scopes ad hoc meaning that the delegate won't run
					//until the scope or its inheritor is requested to resolve the dependency the delegate is registered to
					//
					//Maybe that's a form of lazy initialization but that shit's annoying as FUCK because I may be expecting
					//the instance created in the delegate to start doing shit the moment it's created and that moment is expected
					//to happen RIGHT NOW, at the very least - when the scoped lifetime is created, NOT when dependency is injected
					//
					//The dependency may not be fucking resolved at all - say I register a service and I want this service to do its
					//thing on its own without injecting it as a dependency to anyone. I still want it to be registered in the
					//DI container for purpose of keeping initialization in the composition root space. The service needs to be created,
					//registered, started up, hooked up to messages from the respective event bus and stand by until an event arrives
					//
					//That said, to counter this STUPID ass bug I shall trail my delegate registrations with callbacks that do nothing
					//but try to resolve the dependencies without making any purpose of them. Those callbacks are triggered after
					//the lifetime scope building is complete so that will ensure the delegates are invoked when I need them to
					containerBuilder
						.RegisterBuildCallback(componentContext =>
						{
							componentContext.TryResolve<IRuntimeResourceManager>(out var resourceManager);
						});
				});

			base.InitializeInternal();
		}

		protected override void CleanupInternal()
		{
			if (parentLifetime
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