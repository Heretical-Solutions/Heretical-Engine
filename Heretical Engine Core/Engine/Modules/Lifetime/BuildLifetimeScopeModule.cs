using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class BuildLifetimeScopeModule
		: ALifetimeModule
	{
		public override string Name => "Build lifetime scope module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;

			compositionRoot.PushLifetimeScope();

			base.InitializeInternal();
		}

		protected override void CleanupInternal()
		{
			var compositionRoot = context as ICompositionRoot;

			compositionRoot.PopLifetimeScope();

			base.CleanupInternal();
		}
	}
}