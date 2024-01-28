using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ILifetimeScopeManager
	{
		ILifetimeScope CurrentLifetimeScope { get; }

		void QueueLifetimeScopeAction(
			Action<ContainerBuilder> lifetimeScopeAction);

		void PushLifetimeScope();

		void PopLifetimeScope();
	}
}