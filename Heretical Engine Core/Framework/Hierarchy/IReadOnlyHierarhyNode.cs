namespace HereticalSolutions.Hierarchy
{
	public interface IReadOnlyHierarchyNode
	{
		bool IsRoot { get; }

		IReadOnlyHierarchyNode Parent { get; }

		IEnumerable<IReadOnlyHierarchyNode> Children { get; }
	}
}