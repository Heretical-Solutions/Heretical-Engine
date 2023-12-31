using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
	[Component("Hierarchy")]
	public struct HierarchyComponent
	{
		public Entity Parent;

		public int ChildrenListID;
	}
}