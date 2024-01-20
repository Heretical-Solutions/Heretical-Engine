using HereticalSolutions.HereticalEngine.Modules;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IApplicationContext
	{
		IContainer Container { get; }

		ILifetimeScope CurrentScope { get; }

		IEnumerable<IModule> ActiveModules { get; }
	}
}