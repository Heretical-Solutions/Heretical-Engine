using HereticalSolutions.HereticalEngine.Modules;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IModuleManager
	{
		void AddActiveModule(IModule module);

		void RemoveActiveModule(IModule module);

		void LoadModule(IModule module);

		void UnloadModule(IModule module);
	}
}