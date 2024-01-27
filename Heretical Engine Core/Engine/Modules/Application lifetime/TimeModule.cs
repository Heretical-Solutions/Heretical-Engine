using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class TimeModule
		: ALifetimeableModule
	{
		public override string Name => "Time module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;

			var containerBuilder = compositionRoot.ContainerBuilder;

			containerBuilder
				.Register(componentContext =>
				{
					componentContext.TryResolve<ILoggerResolver>(
						out ILoggerResolver loggerResolver);

					var logger = loggerResolver?.GetLogger<TimeModule>();

					logger?.Log<TimeModule>(
						"BUILDING TIME MANAGER");

					ITimeManager timeManager = TimeFactory.BuildTimeManager(loggerResolver);

					return timeManager;
				})
				.As<ITimeManager>()
				.SingleInstance();

			base.InitializeInternal();
		}

		protected override void CleanupInternal()
		{
			if (context
				.CurrentLifetimeScope
				.TryResolve<ITimeManager>(
					out ITimeManager timeManager))
			{
				((ISynchronizablesGenericArgRepository<float>)timeManager).RemoveAllSynchronizables();
			}

			base.CleanupInternal();
		}
	}
}