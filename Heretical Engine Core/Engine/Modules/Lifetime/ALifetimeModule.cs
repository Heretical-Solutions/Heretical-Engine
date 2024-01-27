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
			var lifetimeComposer = context as ILifetimeComposer;

			var lifetimeScopeManager = context as ILifetimeScopeManager;

			parentLifetime = context.CurrentLifetime;

			isRootLifetime = parentLifetime == null;

			lifetimeComposer.SetLifetimeAsCurrent(
				this,
				isRootLifetime);

			lifetimeScopeManager.QueueLifetimeScopeAction(
				containerBuilder =>
				{
					containerBuilder
						.Register(componentContext =>
						{
							return this;
						})
						//.RegisterInstance(this)
						.Named<ILifetimeable>(ModuleLifetimeConstants.CURRENT_SCOPE)
						.SingleInstance();

					containerBuilder
						.Register(componentContext =>
						{
							return this;
						})
						//.RegisterInstance(this)
						.Named<ILifetimeable>(Name)
						.SingleInstance();
				});

			base.InitializeInternal();
		}

        protected override void CleanupInternal()
        {
			var lifetimeComposer = context as ILifetimeComposer;

			lifetimeComposer.SetLifetimeAsCurrent(
				parentLifetime,
				isRootLifetime);

			parentLifetime = null;

			isRootLifetime = false;

			base.CleanupInternal();
		}
    }
}