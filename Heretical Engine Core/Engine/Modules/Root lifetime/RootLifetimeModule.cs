using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Hierarchy;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class RootLifetimeModule
		: ALifetimeModule
	{
		public override string Name => "Root lifetime module";

		public RootLifetimeModule(
			List<IReadOnlyHierarchyNode> children,
			List<Action<ContainerBuilder>> lifetimeScopeActions,
			IModule[] initialModules)
			: base(
				children,
				lifetimeScopeActions,
				initialModules)
		{
		}

		protected override void InitializeInternal()
		{
			//Good boy Autopilot, good boy
			var rootLifetimeManager = context as IRootLifetimeManager;

			rootLifetimeManager.SetRootLifetime(this);

			base.InitializeInternal();

			TryResolveLogger(null);
		}
	}
}