using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface ICoreModule
		: IModule
	{
		void Run(
			ApplicationContext context);
	}
}