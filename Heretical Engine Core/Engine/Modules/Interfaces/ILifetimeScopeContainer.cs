using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface ILifetimeScopeContainer
	{
		ILifetimeScope CurrentLifetimeScope { get; }

		void QueueLifetimeScopeAction(
			Action<ContainerBuilder> lifetimeScopeAction);
	}
}