using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class BuildLifetimeScopeModule
		: IModule
	{
		public BuildLifetimeScopeModule()
		{
		}

		#region IModule

		public string Name => "Build lifetime scope module";

		public void Load(IApplicationContext context)
		{
			var compositionRoot = context as ICompositionRoot;

			compositionRoot.PushLifetimeScope();
		}

		public void Unload(IApplicationContext context)
		{
			//Moded to ALifetimeModule
			/*
			var compositionRoot = context as ICompositionRoot;

			compositionRoot.PopLifetimeScope();
			*/
		}

		#endregion
	}
}