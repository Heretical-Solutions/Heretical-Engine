using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class SynchronizationModule
		: ALifetimeableModule
	{
		public SynchronizationModule(ILogger logger = null)
			: base(logger)
		{
		}

		public override string Name => "Synchronization module";

		public override void Load(IApplicationContext context)
		{
			var contextAsCompositionRoot = context as ICompositionRoot;

			var containerBuilder = contextAsCompositionRoot.ContainerBuilder;

			containerBuilder
				.Register(componentContext =>
				{
					ISynchronizationManager synchronizationManager = SynchronizationFactory.BuildSynchronizationManager();

					return synchronizationManager;
				})
			.As<ISynchronizationManager>();

			base.Load(context);
		}

		public override void Unload(IApplicationContext context)
		{
			//TODO: UNSUBSCRIBE ALL

			base.Unload(context);
		}
	}
}