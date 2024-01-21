using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Synchronization;

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

			ITimeManager timeManager = TimeFactory.BuildTimeManager();

			containerBuilder
				.Register(componentContext =>
				{
					return timeManager;
				})
			.As<ITimeManager>();

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