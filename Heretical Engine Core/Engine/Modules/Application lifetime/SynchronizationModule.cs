using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class SynchronizationModule
		: ALifetimeableModule
	{
		public override string Name => "Synchronization module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;

			var containerBuilder = compositionRoot.ContainerBuilder;

			containerBuilder
				.Register(componentContext =>
				{
					ISynchronizationManager synchronizationManager = SynchronizationFactory.BuildSynchronizationManager();

					return synchronizationManager;
				})
			.As<ISynchronizationManager>();

			base.InitializeInternal();
		}

		protected override void CleanupInternal()
		{
			if (context.CurrentLifetimeScope.TryResolve<ISynchronizationManager>(
				out ISynchronizationManager synchronizationManager))
			{
				((ISynchronizablesRepository)synchronizationManager).RemoveAllSynchronizables();
			}

			base.CleanupInternal();
		}
	}
}