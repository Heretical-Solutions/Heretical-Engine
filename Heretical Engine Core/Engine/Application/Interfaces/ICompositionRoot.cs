using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ICompositionRoot
	{
		IContainer DIContainer { get; }

		ContainerBuilder ContainerBuilder { get; }

		void BuildContainer();
	}
}