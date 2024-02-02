using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Time;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ApplicationTimePointsModule
		: ALifetimeableModule
	{
		public override string Name => "Application time points module";

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
								.TryResolve<ITimeManager>(
									out var timeManager))
							{
								throw new Exception(
									logger.TryFormat<ApplicationTimePointsModule>(
										"COULD NOT RESOLVE TIME MANAGER"));
							}

							logger?.Log<ApplicationTimePointsModule>(
								"BUILDING APPLICATION TIME POINTS");

							var synchronizablesRepository = timeManager
								as ISynchronizablesGenericArgRepository<float>;

							synchronizablesRepository.AddSynchronizable(
								SynchronizationFactory.BuildFixedDeltaSynchronizationContextGeneric<float>(
									ApplicationTimeConstants.FIXED_UPDATE,
									fixedDelta: (1f / 60f),
									deltaToFloatDelegate: delta => delta,
									canBeToggled: true,
									active: true,
									canScale: true,
									scale: 1f,
									scaleDeltaDelegate: (value, scale) => value * scale,
									loggerResolver: loggerResolver));

							synchronizablesRepository.AddSynchronizable(
								SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
									ApplicationTimeConstants.UPDATE,
									canBeToggled: true,
									active: true,
									canScale: true,
									scale: 1f,
									scaleDeltaDelegate: (value, scale) => value * scale,
									loggerResolver: loggerResolver));

							synchronizablesRepository.AddSynchronizable(
								SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
									ApplicationTimeConstants.LATE_UPDATE,
									canBeToggled: true,
									active: true,
									canScale: true,
									scale: 1f,
									scaleDeltaDelegate: (value, scale) => value * scale,
									loggerResolver: loggerResolver));
						});
				});

			base.InitializeInternal();
		}
	}
}