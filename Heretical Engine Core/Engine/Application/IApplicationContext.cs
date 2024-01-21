using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.LifetimeManagement;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IApplicationContext
	{
		IContainer DIContainer { get; }

		ILifetimeScope CurrentLifetimeScope { get; }

		ILifetimeable RootLifetime { get; }

		ILifetimeable CurrentLifetime { get; }

		IEnumerable<IModule> ActiveModules { get; }
	}
}