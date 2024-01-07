using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Metadata.Factories
{
	public static partial class MetadataFactory
	{
		public static IReadOnlyObjectRepository BuildTypeToObjectMetadataRepository(
			MetadataAllocationDescriptor[] metadataDescriptors)
		{
			var repository = RepositoriesFactory.BuildDictionaryObjectRepository();

			if (metadataDescriptors != null)
				foreach (var descriptor in metadataDescriptors)
				{
					if (descriptor == null)
						continue;

					repository.Add(
						descriptor.BindingType,
						AllocationsFactory.ActivatorAllocationDelegate(descriptor.ConcreteType));
				}

			return repository;
		}
	}
}