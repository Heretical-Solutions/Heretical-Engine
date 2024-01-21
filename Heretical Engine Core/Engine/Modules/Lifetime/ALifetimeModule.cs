using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.LifetimeManagement;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public abstract class ALifetimeModule
		: ALifetimeableModule,
		  IModule
	{
		public override string Name => "Abstract lifetime module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;
			
			compositionRoot.SetLifetimeAsCurrent(this);

			compositionRoot.QueueLifetimeScopeAction(
				containerBuilder =>
				{
					containerBuilder
						.Register(componentContext =>
						{
							return this;
						})
					.Named<ILifetimeable>(ModuleLifetimeConstants.CURRENT_SCOPE);

					containerBuilder
						.Register(componentContext =>
						{
							return this;
						})
					.Named<ILifetimeable>(Name);
				});

			base.InitializeInternal();
		}

        protected override void CleanupInternal()
        {
			var compositionRoot = context as ICompositionRoot;

			//Moved here because we want to pop lifetime scope AFTER the current lifetime is torn down
			compositionRoot.PopLifetimeScope();

			base.CleanupInternal();
		}
    }
}