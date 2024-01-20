using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface IModule
		//: ILifetimeable //Actually there's no need for this
	{
		string Name { get; }

		void Load(
			IApplicationContext context);

		void Unload(
			IApplicationContext context);
	}
}