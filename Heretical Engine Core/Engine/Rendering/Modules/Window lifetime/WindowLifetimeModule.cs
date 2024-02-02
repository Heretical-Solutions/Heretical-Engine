using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Hierarchy;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class WindowLifetimeModule
		: ALifetimeModule
	{
		public override string Name => "Window lifetime module";

		public WindowLifetimeModule(
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