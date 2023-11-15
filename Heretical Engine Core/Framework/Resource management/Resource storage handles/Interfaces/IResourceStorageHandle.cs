using System;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
	public interface IResourceStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		bool SetRawResource(object rawResource);

		bool SetResource<TValue>(TValue resource);
	}
}