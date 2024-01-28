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

			parentLifetime = lifetimeComposer.CurrentLifetime;

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

						//For some fucking reason autofac performs delegates in lifetime scopes ad hoc meaning that the delegate won't run
						//until the scope or its inheritor is requested to resolve the dependency the delegate is registered to
						//
						//Maybe that's a form of lazy initialization but that shit's annoying as FUCK because I may be expecting
						//the instance created in the delegate to start doing shit the moment it's created and that moment is expected
						//to happen RIGHT NOW, at the very least - when the scoped lifetime is created, NOT when dependency is injected
						//
						//The dependency may not be fucking resolved at all - say I register a service and I want this service to do its
						//thing on its own without injecting it as a dependency to anyone. I still want it to be registered in the
						//DI container for purpose of keeping initialization in the composition root space. The service needs to be created,
						//registered, started up, hooked up to messages from the respective event bus and stand by until an event arrives
						//
						//That said, to counter this STUPID ass bug I shall trail my delegate registrations with callbacks that do nothing
						//but try to resolve the dependencies without making any purpose of them. Those callbacks are triggered after
						//the lifetime scope building is complete so that will ensure the delegates are invoked when I need them to
						containerBuilder
							.RegisterBuildCallback(componentContext =>
							{
								componentContext.TryResolveNamed<ILifetimeable>(
									ModuleLifetimeConstants.CURRENT_SCOPE,
									out var lifetimeable1);

								componentContext.TryResolveNamed<ILifetimeable>(
									Name,
									out var lifetimeable2);
							});
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