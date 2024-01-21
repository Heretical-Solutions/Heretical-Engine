using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ILifetimeScopeManager
	{
		void QueueLifetimeScopeAction(
			Action<ContainerBuilder> lifetimeScopeAction);

		void PushLifetimeScope();

		void PopLifetimeScope();
	}
}