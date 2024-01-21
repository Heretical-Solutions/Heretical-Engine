using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.LifetimeManagement;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ICompositionRoot
	{
		ContainerBuilder ContainerBuilder { get; }

		void BuildContainer();
	}
}