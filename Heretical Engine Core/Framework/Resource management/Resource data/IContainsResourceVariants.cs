using System.Collections.Generic;

namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsResourceVariants
	{
		IResourceVariantData DefaultVariant { get; }

		bool HasVariant(int variantIDHash);

		bool HasVariant(string variantID);

		IResourceVariantData GetVariant(int variantIDHash);

		IResourceVariantData GetVariant(string variantID);

		bool TryGetVariant(int variantIDHash, out IResourceVariantData variant);

		bool TryGetVariant(string variantID, out IResourceVariantData variant);

		IEnumerable<int> VariantIDHashes { get; }

		IEnumerable<string> VariantIDs { get; }

		IEnumerable<IResourceVariantData> AllVariants { get; }
	}
}