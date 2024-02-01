using HereticalSolutions.Hierarchy;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ApplicationLifetimeModule
		: ALifetimeModule
	{
		public override string Name => "Application lifetime module";

		public ApplicationLifetimeModule(
			List<IReadOnlyHierarchyNode> children,
			List<Action<ContainerBuilder>> lifetimeScopeActions,
			IModule[] initialModules)
			: base(
				children,
				lifetimeScopeActions,
				initialModules)
		{
		}
	}
}