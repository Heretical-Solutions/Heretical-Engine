using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class RenderingSynchronizationPointsModule
		: ALifetimeableModule
	{
		public override string Name => "Rendering synchronization points module";

		protected override void InitializeInternal()
		{
			var lifetimeScopeManager = context as ILifetimeScopeManager;

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
									logger.TryFormat<RenderingSynchronizationPointsModule>(
										"COULD NOT RESOLVE SYNCHRONIZATION MANAGER"));
							}

							logger?.Log<RenderingSynchronizationPointsModule>(
								"BUILDING RENDERING SYNCHRONIZATION POINTS");

							((ISynchronizablesRepository)synchronizationManager)
								.AddSynchronizable(
									SynchronizationFactory.BuildSynchronizationContext(
										RenderingSynchronizationConstants.RENDER,
										false,
										loggerResolver));
						});
				});

			base.InitializeInternal();
		}
	}
}