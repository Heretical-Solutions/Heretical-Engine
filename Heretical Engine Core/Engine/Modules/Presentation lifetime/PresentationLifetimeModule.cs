using HereticalSolutions.Hierarchy;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class PresentationLifetimeModule
		: ALifetimeModule
	{
		public override string Name => "Presentation lifetime module";

		public PresentationLifetimeModule(
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