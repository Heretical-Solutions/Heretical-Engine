namespace HereticalSolutions.Hierarchy
{
	public interface IHierarchyNode
		: IReadOnlyHierarchyNode
	{
		void SetParent(
			IReadOnlyHierarchyNode parent,
			bool addAsChild = true);

		void RemoveFromParent(bool removeAsChild = true);


		void AddChild(
			IReadOnlyHierarchyNode child,
			bool setParent = true);

		void RemoveChild(
			IReadOnlyHierarchyNode child,
			bool removeParent = true);

		void RemoveAllChildren(bool removeParents = true);
	}
}