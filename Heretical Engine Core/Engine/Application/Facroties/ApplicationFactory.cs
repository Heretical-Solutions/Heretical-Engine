using HereticalSolutions.HereticalEngine.Modules;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public static class ApplicationFactory
	{
		public static ApplicationContext BuildApplicationContext()
		{
			return new ApplicationContext(
				new ContainerBuilder(),
				new List<IModule>());
		}
	}
}