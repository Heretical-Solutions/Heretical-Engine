using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Hierarchy;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public abstract class ALifetimeModule
		: ALifetimeableModule,
		  ILifetimeModule
	{
		protected readonly List<IReadOnlyHierarchyNode> children;

		protected readonly List<Action<ContainerBuilder>> lifetimeScopeActions;

		protected readonly IModule[] initialModules;

		protected ILifetimeScope lifetimeScope;

		public override string Name => "Abstract lifetime module";

		public ALifetimeModule(
			List<IReadOnlyHierarchyNode> children,
			List<Action<ContainerBuilder>> lifetimeScopeActions,
			IModule[] initialModules)
		{
			this.children = children;

			this.lifetimeScopeActions = lifetimeScopeActions;

			this.initialModules = initialModules;

			lifetimeScope = null;
		}

		#region IHierarchyNode

		#region IReadOnlyHierarchyNode

		public bool IsRoot { get => parentLifetime != null; }

		public IReadOnlyHierarchyNode Parent { get => parentLifetime; }

		public IEnumerable<IReadOnlyHierarchyNode> Children { get => children; }

		#endregion

		public void SetParent(
			IReadOnlyHierarchyNode parent,
			bool addAsChild = true)
		{
			ILifetimeModule parentModule = parent as ILifetimeModule;

			parentLifetime = parentModule;

			if (addAsChild)
				((IHierarchyNode)parent)?.AddChild(
					this,
					false);
		}

		public void RemoveFromParent(bool removeAsChild = true)
		{
			if (removeAsChild)
				((IHierarchyNode)parentLifetime)?.RemoveChild(
					this,
					false);

			parentLifetime = null;
		}


		public void AddChild(
			IReadOnlyHierarchyNode child,
			bool setParent = true)
		{
			children.Add(child);

			if (setParent)
				((IHierarchyNode)child)?.SetParent(
					this,
					false);
		}

		public void RemoveChild(
			IReadOnlyHierarchyNode child,
			bool removeParent = true)
		{
			if (removeParent)
				((IHierarchyNode)child)?.RemoveFromParent(false);

			children.Remove(child);
		}

		public void RemoveAllChildren(bool removeParents = true)
		{
			if (removeParents)
				foreach (var child in children)
					((IHierarchyNode)child)?.RemoveFromParent(false);

			children.Clear();
		}

		#endregion

		#region ILifetimeScopeContainer

		public ILifetimeScope CurrentLifetimeScope { get => lifetimeScope; }

		public void QueueLifetimeScopeAction(Action<ContainerBuilder> lifetimeScopeAction)
		{
			lifetimeScopeActions.Add(lifetimeScopeAction);
		}

		#endregion

		protected override void InitializeInternal()
		{
			foreach (var initialModule in initialModules)
			{
				var moduleManager = context as IModuleManager;

				moduleManager.LoadModule(initialModule, this);
			}

			if (parentLifetime == null)
			{
				BuildDIContainerIfRoot();
			}

			RegisterSelf();

			BuildLifetimeScope();

			base.InitializeInternal();
		}

		private void RegisterSelf()
		{
			QueueLifetimeScopeAction(
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
		}

		private void BuildDIContainerIfRoot()
		{
			var compositionRoot = context as ICompositionRoot;

			compositionRoot.BuildContainer();
		}

		private void BuildLifetimeScope()
		{
			var currentLifetimeScopeActions = lifetimeScopeActions.ToArray();

			Action<ContainerBuilder> configurationAction =
				(currentContainerBuilder) =>
				{
					foreach (var action in currentLifetimeScopeActions)
					{
						action?.Invoke(currentContainerBuilder);
					}
				};

			var parentAsLifetimeScopeContainer = parentLifetime as ILifetimeScopeContainer;

			if (parentAsLifetimeScopeContainer != null)
			{
				lifetimeScope = parentAsLifetimeScopeContainer
					.CurrentLifetimeScope
					.BeginLifetimeScope(configurationAction);
			}
			else
			{
				var compositionRoot = context as ICompositionRoot;

				lifetimeScope = compositionRoot
					.DIContainer
					.BeginLifetimeScope(configurationAction);
			}

			lifetimeScopeActions.Clear();
		}

		/*
        protected override void CleanupInternal()
        {
			lifetimeScope.Dispose();

			base.CleanupInternal();
		}
		*/

		//Here instead of CleanupInternal because the modules may try to clean up their stuff by resolving things they registered first
		//And if we do lifetimeScope.Dispose(); in Cleanup then they end up with a lifetime that is basically garbage now
		public virtual void TearDown()
		{
			base.TearDown();

			lifetimeScope.Dispose();
		}
	}
}