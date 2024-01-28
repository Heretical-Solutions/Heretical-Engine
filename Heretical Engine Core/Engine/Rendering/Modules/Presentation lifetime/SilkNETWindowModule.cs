using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Delegates;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Autofac;
using HereticalSolutions.Delegates.Factories;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class SilkNETWindowModule
		: ALifetimeableModule
	{
		public override string Name => "Silk.NET window module";

		protected override void InitializeInternal()
		{
			var lifetimeScopeManager = context as ILifetimeScopeManager;

			lifetimeScopeManager.QueueLifetimeScopeAction(
				containerBuilder =>
				{
					containerBuilder
						.Register(componentContext =>
						{
							componentContext
								.TryResolve<ILoggerResolver>(
									out var loggerResolver);

							if (!componentContext
								.TryResolve<ConcurrentGenericCircularBuffer<MainThreadCommand>>(
									out var mainThreadCommandBuffer))
							{
								throw new Exception(
									logger.TryFormat<SilkNETWindowModule>(
										"COULD NOT RESOLVE MAIN THREAD COMMAND BUFFER"));
							}

							if (!componentContext
								.TryResolve<ISynchronizationManager>(
									out var synchronizationManager))
							{
								throw new Exception(
									logger.TryFormat<SilkNETWindowModule>(
										"COULD NOT RESOLVE SYNCHRONIZATION MANAGER"));
							}

							if (!componentContext
								.TryResolve<ITimeManager>(
									out var timeManager))
							{
								throw new Exception(
									logger.TryFormat<SilkNETWindowModule>(
										"COULD NOT RESOLVE TIME MANAGER"));
							}

							if (!componentContext
								.TryResolveNamed<ITimeManager>(
									RenderingConstants.RENDERING_TIME_MANAGER_NAME,
									out var renderingTimeManager))
							{
								throw new Exception(
									logger.TryFormat<SilkNETWindowModule>(
										"COULD NOT RESOLVE TIME MANAGER"));
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

								synchronizationManager
									.SynchronizeAll(ApplicationSynchronizationConstants.UPDATE);

								timeManager
									.Tick((float)timeDelta);
							};

							window.Render += timeDelta =>
							{
								synchronizationManager
									.SynchronizeAll(RenderingSynchronizationConstants.RENDER);

								renderingTimeManager
									.Tick((float)timeDelta);
							};

							// The closing function
							window.Closing += () =>
							{
								synchronizationManager
									.SynchronizeAll(WindowSynchronizationConstants.WINDOW_UNLOADED);

								logger?.Log<SilkNETWindowModule>(
									"WINDOW UNLOADED");
							};

							// Now that everything's defined, let's run this bad boy!\
							//UPDATE: NOT now. After all the modules are loaded

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

							return window;
						})
						.As<IWindow>()
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
							componentContext.TryResolve<IWindow>(out var window);
						});
				});

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
	}
}