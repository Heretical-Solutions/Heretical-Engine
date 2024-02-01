using HereticalSolutions.HereticalEngine.Modules;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IApplicationContext
	{
		EApplicationStatus CurrentStatus { get; }

		IEnumerable<IModule> ActiveModules { get; }

		ILifetimeModule RootLifetime { get; }
	}
}