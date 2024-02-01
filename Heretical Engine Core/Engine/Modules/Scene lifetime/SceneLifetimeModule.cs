using HereticalSolutions.Hierarchy;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class SceneLifetimeModule
		: ALifetimeModule
	{
		public override string Name => "Scene lifetime module";

		public SceneLifetimeModule(
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