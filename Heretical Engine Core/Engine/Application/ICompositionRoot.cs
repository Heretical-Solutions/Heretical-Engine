using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ICompositionRoot
	{
		public ContainerBuilder ContainerBuilder { get; }

		void BuildContainer();

		void PushLifetimeScope(
			Action<ContainerBuilder> configurationAction);

		void PopLifetimeScope();
	}
}