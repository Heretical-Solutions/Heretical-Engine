using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.LifetimeManagement;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ICompositionRoot
	{
		ContainerBuilder ContainerBuilder { get; }

		void BuildContainer();


		void NestLifetime(ILifetimeable lifetime);

		void SetLifetimeAsCurrent(ILifetimeable lifetime);


		void QueueLifetimeScopeAction(
			Action<ContainerBuilder> lifetimeScopeAction);

		void PushLifetimeScope();

		void PopLifetimeScope();


		void AddActiveModule(IModule module);

		void RemoveActiveModule(IModule module);

		void LoadModule(IModule module);

		void UnloadModule(IModule module);
	}
}