using HereticalSolutions.HereticalEngine.Modules;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IApplicationContext
	{
		public IContainer Container { get; }

		ILifetimeScope CurrentScope { get; }

		public IEnumerable<IModule> ActiveModules { get; }
	}
}