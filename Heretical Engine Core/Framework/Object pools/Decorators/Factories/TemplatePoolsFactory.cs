using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools.Allocations;

namespace HereticalSolutions.Pools.Factories
{
	public static partial class PoolsFactory
	{
		public static INonAllocDecoratedPool<TValue> BuildSimpleResizableObjectPool<TValue>(
			AllocationCommandDescriptor initialAllocation,
			AllocationCommandDescriptor additionalAllocation,
			object[] valueAllocationArguments = null)
		{
			#region Builders

			var resizablePoolBuilder = new ResizablePoolBuilder<TValue>();

			#endregion

			#region Metadata descriptor builders

			var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
			{
				PoolsFactory.BuildIndexedMetadataDescriptor
			};

			#endregion

			#region Value allocation delegate initialization

			Func<TValue> valueAllocationDelegate;

			if (valueAllocationArguments == null)
			{
				valueAllocationDelegate = () => AllocationsFactory.ActivatorAllocationDelegate<TValue>();
			}
			else
			{
				valueAllocationDelegate = () => AllocationsFactory.ActivatorAllocationDelegate<TValue>(valueAllocationArguments);
			}

			#endregion

			resizablePoolBuilder.Initialize(
				valueAllocationDelegate,
				metadataDescriptorBuilders,
				initialAllocation,
				additionalAllocation,
				null);

			var resizablePool = resizablePoolBuilder.BuildResizablePool();

			return resizablePool;
		}

		public static INonAllocDecoratedPool<TAbstractValue> BuildSimpleResizableObjectPool<TAbstractValue, TConcreteValue>(
			AllocationCommandDescriptor initialAllocation,
			AllocationCommandDescriptor additionalAllocation,
			object[] valueAllocationArguments = null)
		{
			#region Builders

			var resizablePoolBuilder = new ResizablePoolBuilder<TAbstractValue>();

			#endregion

			#region Metadata descriptor builders

			var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
			{
				PoolsFactory.BuildIndexedMetadataDescriptor
			};

			#endregion

			#region Value allocation delegate initialization

			Func<TAbstractValue> valueAllocationDelegate;

			if (valueAllocationArguments == null)
			{
				valueAllocationDelegate = () => AllocationsFactory.ActivatorAllocationDelegate<TAbstractValue, TConcreteValue>();
			}
			else
			{
				valueAllocationDelegate = () => AllocationsFactory.ActivatorAllocationDelegate<TAbstractValue, TConcreteValue>(valueAllocationArguments);
			}

			#endregion

			resizablePoolBuilder.Initialize(
				valueAllocationDelegate,
				metadataDescriptorBuilders,
				initialAllocation,
				additionalAllocation,
				null);

			var resizablePool = resizablePoolBuilder.BuildResizablePool();

			return resizablePool;
		}
	}
}