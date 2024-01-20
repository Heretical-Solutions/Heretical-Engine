using HereticalSolutions.HereticalEngine.Application;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ApplicationDataModule
		: IModule
	{
		public ApplicationDataModule()
		{
		}

		#region IModule

		public string Name => "Application data module";

		public void Load(IApplicationContext context)
		{
			var contextAsCompositionRoot = context as ICompositionRoot;

			var containerBuilder = contextAsCompositionRoot.ContainerBuilder;


			var pathToExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

			//TODO: change
			var applicationDataFolder = pathToExe.Substring(
				0,
				pathToExe.IndexOf("/bin/"));


			containerBuilder
				.Register(componentContext =>
				{
					return applicationDataFolder;
				})
				.Named<string>("Application data folder");
		}

		public void Unload(IApplicationContext context)
		{
		}

		#endregion
	}
}