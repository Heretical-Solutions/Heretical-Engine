using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ApplicationSynchronizationPointsModule
		: ALifetimeableModule
	{
		public override string Name => "Application synchronization points module";

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
									logger.TryFormat<ApplicationSynchronizationPointsModule>(
										"COULD NOT RESOLVE SYNCHRONIZATION MANAGER"));
							}

							logger?.Log<ApplicationSynchronizationPointsModule>(
								"BUILDING APPLICATION SYNCHRONIZATION POINTS");

							((ISynchronizablesRepository)synchronizationManager)
								.AddSynchronizable(
									SynchronizationFactory.BuildSynchronizationContext(
										ApplicationSynchronizationConstants.START_APPLICATION,
										false,
										loggerResolver));

							((ISynchronizablesRepository)synchronizationManager)
								.AddSynchronizable(
									SynchronizationFactory.BuildSynchronizationContext(
										ApplicationSynchronizationConstants.SHUT_DOWN_APPLICATION,
										false,
										loggerResolver));

							((ISynchronizablesRepository)synchronizationManager)
								.AddSynchronizable(
									SynchronizationFactory.BuildSynchronizationContext(
										ApplicationSynchronizationConstants.UPDATE,
										false,
										loggerResolver));
						});
				});

			base.InitializeInternal();
		}
	}
}