using System.Collections.Generic;

namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsNestedResources
	{
		bool HasNestedResource(int nestedResourceIDHash);

		bool HasNestedResource(string nestedResourceID);

		IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash);

		IReadOnlyResourceData GetNestedResource(string nestedResourceIDHash);

		IEnumerable<int> NestedResourceIDHashes { get; }

		IEnumerable<string> NestedResourceIDs { get; }

		IEnumerable<IReadOnlyResourceData> AllNestedResources { get; }
	}
}