using System;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
	public interface IReadOnlyResourceStorageHandle
		: IAllocatable
	{
		object RawResource { get; }

		TValue GetResource<TValue>();
	}
}