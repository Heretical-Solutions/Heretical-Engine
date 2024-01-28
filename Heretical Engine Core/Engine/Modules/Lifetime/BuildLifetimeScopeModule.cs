using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	//TODO: branch instead of pushing / popping
	public class BuildLifetimeScopeModule
		: ALifetimeableModule
	{
		public override string Name => "Build lifetime scope module";

		protected override void InitializeInternal()
		{
			var lifetimeScopeManager = context as ILifetimeScopeManager;

			lifetimeScopeManager.PushLifetimeScope();

			base.InitializeInternal();
		}

		protected override void CleanupInternal()
		{
			var lifetimeScopeManager = context as ILifetimeScopeManager;

			lifetimeScopeManager.PopLifetimeScope();

			base.CleanupInternal();
		}
	}
}