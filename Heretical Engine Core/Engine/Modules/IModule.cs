using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface IModule
		: ILifetimeable
	{
		string Name { get; }

		void Load(
			IApplicationContext context);

		void Unload(
			IApplicationContext context);
	}
}