using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Time;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class RenderingTimePointsModule
		: ALifetimeableModule
	{
		public override string Name => "Rendering time points module";

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
								.TryResolveNamed<ITimeManager>(
									RenderingConstants.RENDERING_TIME_MANAGER_NAME,
									out var renderingTimeManager))
							{
								throw new Exception(
									logger.TryFormat<RenderingTimePointsModule>(
										"COULD NOT RESOLVE RENDERING TIME MANAGER"));
							}

							logger?.Log<RenderingTimePointsModule>(
								"BUILDING RENDERING TIME POINTS");

							var synchronizablesRepository = renderingTimeManager
								as ISynchronizablesGenericArgRepository<float>;

							synchronizablesRepository.AddSynchronizable(
								SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
									RenderingSynchronizationConstants.RENDER,
									canBeToggled: false,
									canScale: false,
									loggerResolver: loggerResolver));
						});
				});

			base.InitializeInternal();
		}
	}
}