using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class WindowSynchronizationPointsModule
		: ALifetimeableModule
	{
		public override string Name => "Window synchronization points module";

		protected override void InitializeInternal()
		{
			var lifetimeScopeManager = parentLifetime as ILifetimeScopeContainer;

			lifetimeScopeManager.QueueLifetimeScopeAction(
				containerBuilder =>
				{
					containerBuilder
						.RegisterBuildCallback(componentContext =>
						{
							componentContext
								.TryResolve<ILoggerResolver>(
									out var loggerResolver);

							if (!componentContext
								.TryResolve<ISynchronizationManager>(
									out var synchronizationManager))
							{
								throw new Exception(
									logger.TryFormat<WindowSynchronizationPointsModule>(
										"COULD NOT RESOLVE SYNCHRONIZATION MANAGER"));
							}

							logger?.Log<WindowSynchronizationPointsModule>(
								"BUILDING WINDOW SYNCHRONIZATION POINTS");

							((ISynchronizablesRepository)synchronizationManager)
								.AddSynchronizable(
									SynchronizationFactory.BuildSynchronizationContext(
										WindowSynchronizationConstants.WINDOW_LOADED,
										false,
										loggerResolver));

							((ISynchronizablesRepository)synchronizationManager)
								.AddSynchronizable(
									SynchronizationFactory.BuildSynchronizationContext(
										WindowSynchronizationConstants.WINDOW_UNLOADED,
										false,
										loggerResolver));
						});
				});

			base.InitializeInternal();
		}
	}
}