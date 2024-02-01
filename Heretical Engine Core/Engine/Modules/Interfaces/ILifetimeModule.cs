using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Hierarchy;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface ILifetimeModule
		: IModule,
		  ILifetimeable,
		  IReadOnlyHierarchyNode,
		  ILifetimeScopeContainer
	{
	}
}