using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class DIContainerBuilderModule
		: IModule
	{
		public DIContainerBuilderModule()
		{
		}

		#region IModule

		public string Name => "DI container builder module";

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