using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Silk.NET.OpenGL;

using Silk.NET.Maths;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class OpenGLModule
		: ALifetimeableModule
	{
		public override string Name => "OpenGL module";

		protected override void InitializeInternal()
		{
			var lifetimeScopeManager = parentLifetime as ILifetimeScopeContainer;

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
								.TryResolveNamed<ITimeManager>(
									RenderingConstants.RENDERING_TIME_MANAGER_NAME,
									out var renderingTimeManager))
							{
								throw new Exception(
									logger.TryFormat<SilkNETWindowModule>(
										"COULD NOT RESOLVE TIME MANAGER"));
							}

							if (!componentContext
								.TryResolve<IWindow>(
									out var window))
							{
								throw new Exception(
									logger.TryFormat<OpenGLModule>(
										"COULD NOT RESOLVE WINDOW"));
							}

							logger?.Log<OpenGLModule>(
								"BUILDING OPENGL");

							var gl = window.CreateOpenGL();

							window.FramebufferResize += (newSize) => Resize(
								gl,
								newSize);

							var synchronizationProviderRepository = renderingTimeManager as ISynchronizationProvidersRepository;

							if (!synchronizationProviderRepository.TryGetProvider(
								RenderingSynchronizationConstants.RENDER,
								out var renderSynchronizationProvider))
							{
								throw new Exception(
									logger.TryFormat<OpenGLModule>(
										$"COULD NOT GET RENDER SYNCHRONIZATION PROVIDER"));
							}

							ISubscription renderSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(
								(timeDelta) =>
								{
									Draw(
										gl,
										timeDelta);
								},
								loggerResolver);

							renderSynchronizationProvider.Subscribe(renderSubscription);

							return gl;
						})
						.As<GL>()
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
							componentContext.TryResolve<GL>(
								out var gl);
						});
				});

			base.InitializeInternal();
		}

		private void Draw(
			GL gl,
			float timeDelta)
		{
			EnableDepthBuffer(gl);

			EnableBackfaceCulling(gl);

			EnableScissorTest(gl);

			EnableBlend(gl);

			gl.PolygonMode(
				TriangleFace.FrontAndBack,
				PolygonMode.Fill);

			Clear(gl);

			logger?.Log<OpenGLModule>(
				$"RENDERING. TIME DELTA: {timeDelta}");
		}

		private void EnableDepthBuffer(
			GL gl)
		{
			gl.Enable(EnableCap.DepthTest);

			gl.DepthFunc(DepthFunction.Less);
		}

		private void EnableBackfaceCulling(
			GL gl)
		{
			gl.Enable(EnableCap.CullFace);

			gl.CullFace(TriangleFace.Back);
		}

		private void EnableScissorTest(
			GL gl)
		{
			gl.Enable(EnableCap.ScissorTest);
		}

		private void EnableBlend(GL gl)
		{
			gl.Enable(EnableCap.Blend);

			//Is this additive? TODO: figure out
			//gl.BlendEquation(BlendEquationModeEXT.FuncAdd);

			gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		private void Clear(
			GL gl)
		{
			// Here, we just have a blank screen.
			gl.ClearColor(
				System.Drawing.Color.FromArgb(
					255,
					(int)(.45f * 255),
					(int)(.55f * 255),
					(int)(.60f * 255)));

			gl.Clear(
				(uint)(
					ClearBufferMask.ColorBufferBit
					| ClearBufferMask.DepthBufferBit));
		}

		private void Resize(
			GL gl,
			Vector2D<int> newSize)
		{
			// Adjust the viewport to the new window size
			gl.Viewport(newSize);
		}
	}
}