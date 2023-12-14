using Silk.NET.SDL;
using SDLWindow = Silk.NET.SDL.Window;
using SDLRenderer = Silk.NET.SDL.Renderer;
using SDLEvent = Silk.NET.SDL.Event;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public unsafe class Program
	{
		//SDL. WIP as there is no SDL.something, need to adjust everything

		//Courtesy of https://github.com/Alan-love/xenko/blob/master/sources/engine/Stride.Graphics/SDL/Window.cs
		//Thank you Stride3D

		/*
		public static Sdl SDL;

		private Silk.NET.SDL.Window* sdlHandle;

		static void Main(string[] args)
		{
			SDL = Silk.NET.SDL.Sdl.GetApi();

			SDL.Init(Sdl.InitEverything);

			// Pass first mouse event when user clicked on window 
			SDL.SetHint(Sdl.HintMouseFocusClickthrough, "1");

			// Don't leave fullscreen on focus loss
			SDL.SetHint(Sdl.HintVideoMinimizeOnFocusLoss, "0");
		}
		*/

		static void Main(string[] args)
		{
			new SDLSample().Run();
		}
		
		class SDLSample
		{
			public static Sdl SDL;

			SDLWindow* window;

			SDLRenderer* renderer;

			bool running = true;

			public void Run()
			{
				Setup();

				while (running)
				{
					PollEvents();

					Render();
				}

				CleanUp();
			}

			/// <summary>
			/// Setup all of the SDL resources we'll need to display a window.
			/// </summary>
			void Setup()
			{
				SDL = Sdl.GetApi();

				// Initilizes SDL.
				if (SDL.Init(Sdl.InitVideo) < 0)
				{
					Console.WriteLine($"There was an issue initializing SDL. {SDL.GetErrorS()}");
				}

				// Create a new window given a title, size, and passes it a flag indicating it should be shown.
				window = SDL.CreateWindow(
					"SDL .NET 6 Tutorial",
					Sdl.WindowposUndefined,
					Sdl.WindowposUndefined,
					640,
					480,
					(uint)WindowFlags.Shown);

				if ((IntPtr)window == IntPtr.Zero)
				{
					Console.WriteLine($"There was an issue creating the window. {SDL.GetErrorS()}");
				}

				// Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
				renderer = SDL.CreateRenderer(
					window,
					-1,
					(uint)(RendererFlags.Accelerated |
					RendererFlags.Presentvsync));

				if ((IntPtr)renderer == IntPtr.Zero)
				{
					Console.WriteLine($"There was an issue creating the renderer. {SDL.GetErrorS()}");
				}
			}

			/// <summary>
			/// Checks to see if there are any events to be processed.
			/// </summary>
			void PollEvents()
			{
				SDLEvent e = default;

				// Check to see if there are any events and continue to do so until the queue is empty.
				while (SDL.PollEvent(ref e) == 1)
				{
					switch (e.Type)
					{
						case (uint)EventType.Quit:
							running = false;
							break;
					}
				}
			}

			/// <summary>
			/// Renders to the window.
			/// </summary>
			void Render()
			{
				// Sets the color that the screen will be cleared with.
				SDL.SetRenderDrawColor(renderer, 135, 206, 235, 255);

				// Clears the current render surface.
				SDL.RenderClear(renderer);

				// Switches out the currently presented render surface with the one we just did work on.
				SDL.RenderPresent(renderer);
			}

			/// <summary>
			/// Clean up the resources that were created.
			/// </summary>
			void CleanUp()
			{
				SDL.DestroyRenderer(renderer);
				SDL.DestroyWindow(window);
				SDL.Quit();
			}
		}
	}
}