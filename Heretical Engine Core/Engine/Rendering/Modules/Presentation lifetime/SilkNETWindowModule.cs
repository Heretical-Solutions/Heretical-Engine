using System.Linq;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Time;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class SilkNETWindowModule
		: ALifetimeableModule
	{
		private readonly IModule[] windowLifetimeModules;

		public override string Name => "Silk.NET window module";

		public SilkNETWindowModule(IModule[] windowLifetimeModules)
		{
			this.windowLifetimeModules = windowLifetimeModules;
		}

		protected override void InitializeInternal()
		{
			var lifetimeScopeManager = parentLifetime as ILifetimeScopeContainer;

			lifetimeScopeManager
				.CurrentLifetimeScope
				.TryResolve<ILoggerResolver>(
					out var loggerResolver);

			if (!lifetimeScopeManager
				.CurrentLifetimeScope
				.TryResolve<ConcurrentGenericCircularBuffer<MainThreadCommand>>(
					out var mainThreadCommandBuffer))
			{
				throw new Exception(
					logger.TryFormat<SilkNETWindowModule>(
						"COULD NOT RESOLVE MAIN THREAD COMMAND BUFFER"));
			}

			if (!lifetimeScopeManager
				.CurrentLifetimeScope
				.TryResolve<ISynchronizationManager>(
					out var synchronizationManager))
			{
				throw new Exception(
					logger.TryFormat<SilkNETWindowModule>(
						"COULD NOT RESOLVE SYNCHRONIZATION MANAGER"));
			}

			if (!lifetimeScopeManager
				.CurrentLifetimeScope
				.TryResolve<ITimeManager>(
					out var timeManager))
			{
				throw new Exception(
					logger.TryFormat<SilkNETWindowModule>(
						"COULD NOT RESOLVE TIME MANAGER"));
			}

			if (!lifetimeScopeManager
				.CurrentLifetimeScope
				.TryResolveNamed<ITimeManager>(
					RenderingConstants.RENDERING_TIME_MANAGER_NAME,
					out var renderingTimeManager))
			{
				throw new Exception(
					logger.TryFormat<SilkNETWindowModule>(
						"COULD NOT RESOLVE RENDERING TIME MANAGER"));
			}

			logger?.Log<SilkNETWindowModule>(
				"BUILDING WINDOW");

			var windowOptions = WindowOptions.Default;

			//FUCKING HELL!
			//I THOUGHT THAT WITH ENOUGH PATIENCE AND DEDICATION I CAN GET THIS ENTIRE THING TO WORK BY FOLLOWING TUTORIALS AND BE
			//THOROUGH AND CAREFUL. BUT THIS ONE IS OUTRIGHT RIDICULOUS. GUESS WHY MY SUZANNE WAS CLIPPING BOTH OF ITS EARS RIGHT
			//THROUGH ITS HEAD? APPARENTLY, BECAUSE IT SEEMS THAT
			//1. Silk.Net IS NOT ENABLING ITS DEPTH BUFFER BY DEFAULT. THAT WAS AN EASY FIX
			//2. Silk.Net IS NOT SETTING THE WINDOW'S DEPTH BUFFER BITS VALUE AND NOWHERE, NO-FUCKING-WHERE IS IT DOING SO IN THE
			//TUTORIALS THAT I CHECKED OUT. GOD BLESS THIS GUY https://old.reddit.com/r/opengl/comments/z5c4ge/please_help_depth_testing_not_working/ixxirpk/ FOR POINTING THIS OUT
			//3. Silk.Net HAS APPARENTLY FUCKED UP ITS OWN ENUMS OVER TIME SO I HAD SOME HARD TIME FIGURING OUT WHY CullFaceMode ENUM
			//DOES NOT EVEN EXIST
			//FIRST I WAS ANGRY FOR NOT PAYING ENOUGH ATTENTION AT SUCH TRIVIAL THINGS LIKE CASTING ARGUMENT VALUE TO PROPER TYPE TO
			//ENSURE THAT CORRECT OVERLOAD IS SELECTED BUT THIS ONE IS SOMETHING THAT I WOULD BE BEATING MY HEAD AGAINST AND STILL
			//NOT UNDERSTANDING WHAT DID I DO WRONG IF IT WAS NOT FOR SOME GOOD PEOPLE ON STACK OVERFLOW
			windowOptions.PreferredDepthBufferBits = 24;

			//Set up
			var window = Window.Create(windowOptions);

			// Our loading function
			window.Load += () =>
			{
				LoadOpenWindowLifetimeModules(window);

				synchronizationManager
					.SynchronizeAll(WindowSynchronizationConstants.WINDOW_LOADED);

				logger?.Log<SilkNETWindowModule>(
					"WINDOW LOADED");
			};

			window.Update += timeDelta =>
			{
				//WARNING! THIS IMPLIES THAT THE MODULE IS LOADED IN THE MAIN THREAD
				//TODO: ENSURE THE MODULE IS LOADED IN THE MAIN THREAD
				ProcessCommandsOnMainThread(mainThreadCommandBuffer);

				timeManager
					.Tick((float)timeDelta);

				synchronizationManager
					.SynchronizeAll(ApplicationSynchronizationConstants.UPDATE);
			};

			window.Render += timeDelta =>
			{
				renderingTimeManager
					.Tick((float)timeDelta);

				synchronizationManager
					.SynchronizeAll(RenderingSynchronizationConstants.RENDER);
			};

			// The closing function
			window.Closing += () =>
			{
				synchronizationManager
					.SynchronizeAll(WindowSynchronizationConstants.WINDOW_UNLOADED);

				logger?.Log<SilkNETWindowModule>(
					"WINDOW UNLOADED");
			};

			if (!((ISynchronizationProvidersRepository)synchronizationManager)
				.TryGetProvider(
					ApplicationSynchronizationConstants.START_APPLICATION,
					out var startApplicationSynchronizationProvider))
			{
				throw new Exception(
					logger.TryFormat<SilkNETWindowModule>(
						"COULD NOT RESOLVE START APPLICATION SYNCHRONIZATION PROVIDER"));
			}

			ISubscription runSubscription = DelegatesFactory.BuildSubscriptionNoArgs(
				window.Run,
				loggerResolver);

			startApplicationSynchronizationProvider.Subscribe(runSubscription);

			base.InitializeInternal();
		}

		private void ProcessCommandsOnMainThread(
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer)
		{
			while (mainThreadCommandBuffer.TryConsume(out var command))
			{
				if (command.Async)
				{
					var task = command.ExecuteAsync();

					task.Wait();
				}
				else
					command.Execute();
			}
		}

		private void LoadOpenWindowLifetimeModules(IWindow window)
		{
			var moduleManager = context as IModuleManager;

			//Window lifetime
			var windowLifetimeModule = new WindowLifetimeModule(
				new List<IReadOnlyHierarchyNode>(),
				new List<Action<ContainerBuilder>>(),
				windowLifetimeModules
					.Prepend(
						new WindowRegistryModule(window))
					.ToArray());

			moduleManager.LoadModule(
				windowLifetimeModule,
				parentLifetime);
		}
	}
}