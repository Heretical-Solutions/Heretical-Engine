using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class BuildDIContainerModule
		: IModule
	{
		public BuildDIContainerModule()
		{
		}

		#region IModule

		public string Name => "Build DI container module";

		public void Load(IApplicationContext context)
		{
			var compositionRoot = context as ICompositionRoot;

			compositionRoot.BuildContainer();
		}

		public void Unload(IApplicationContext context)
		{
		}

		#endregion
	}
}