using HereticalSolutions.HereticalEngine.Application;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ApplicationDataModule
		: ALifetimeableModule
	{
		public override string Name => "Application data module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;

			var containerBuilder = compositionRoot.ContainerBuilder;


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
				.Named<string>(ApplicationDataConstants.APPLICATION_DATA_FOLDER)
				.SingleInstance();


			base.InitializeInternal();
		}
	}
}