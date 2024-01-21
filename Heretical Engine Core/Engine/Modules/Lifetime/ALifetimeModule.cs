using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.LifetimeManagement;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public abstract class ALifetimeModule
		: ALifetimeableModule,
		  IModule
	{
		protected ILifetimeable parentLifetime;

		protected bool isRootLifetime;

		public override string Name => "Abstract lifetime module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;
			
			parentLifetime = context.CurrentLifetime;

			isRootLifetime = parentLifetime == null;

			compositionRoot.SetLifetimeAsCurrent(
				this,
				isRootLifetime);

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

			compositionRoot.SetLifetimeAsCurrent(
				parentLifetime,
				isRootLifetime);

			parentLifetime = null;

			isRootLifetime = false;

			base.CleanupInternal();
		}
    }
}