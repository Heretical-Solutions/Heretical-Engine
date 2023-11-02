using System.Collections.Generic;

namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsNestedResources
	{
		IReadOnlyResourceData ParentResource { get; }

		bool IsRoot { get; }

		bool HasNestedResource(int nestedResourceIDHash);

		bool HasNestedResource(string nestedResourceID);

		IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash);

		IReadOnlyResourceData GetNestedResource(string nestedResourceID);

		bool TryGetNestedResource(
			int nestedResourceIDHash,
			out IReadOnlyResourceData nestedResource);

		bool TryGetNestedResource(
			string nestedResourceID,
			out IReadOnlyResourceData nestedResource);

		IEnumerable<int> NestedResourceIDHashes { get; }

		IEnumerable<string> NestedResourceIDs { get; }

		IEnumerable<IReadOnlyResourceData> AllNestedResources { get; }
	}
}