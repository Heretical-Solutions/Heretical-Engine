using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ICompositionRoot
	{
		ContainerBuilder ContainerBuilder { get; }

		void BuildContainer();

		void AddPendingContainerAction(Action<ContainerBuilder> containerAction);

		void PushLifetimeScope();

		void PopLifetimeScope();
	}
}