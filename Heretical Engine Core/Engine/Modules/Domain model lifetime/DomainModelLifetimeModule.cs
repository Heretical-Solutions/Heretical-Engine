using HereticalSolutions.Hierarchy;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class DomainModelLifetimeModule
		: ALifetimeModule
	{
		public override string Name => "Domain model lifetime module";

		public DomainModelLifetimeModule(
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