using Silk.NET.Maths;

using Silk.NET.SDL;

using SDLWindow = Silk.NET.SDL.Window;
using SDLRenderer = Silk.NET.SDL.Renderer;
using SDLTexture = Silk.NET.SDL.Texture;
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

			//SDLTexture* texture;

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

				//Texture loading

				//Hello comments section rants, my old friend. I've come to talk with you again after planting my face on another bunch
				//of rakes hidden on my rocky path to create this GLORIOUS engine
				//Turns out that
				//1. A task as simple as loading a fucking texture in SDL2 is a huge pain in the ass
				//2. There is a SEPARATE c lib for that (SDL2_image)
				//3. Silk.NET have NOT added bindings to it (yet?)
				//4. There is only one github repo that does c# bindings of SDL2_image but it also contains bindings to
				//SDL2 itself, along with ANOTHER half a dozen of SEPARATE fucking libs 
				//(https://github.com/flibitijibibo/SDL2-CS/blob/master/src/SDL2_image.cs)
				//5. There is a nuget package with the same name that is authored by entirely different people
				//6. Even after being imported said nuget package is not fucking working
				//7. A tutorial (https://jsayers.dev/c-sharp-sdl-tutorial-part-1-setup/) is generously suggesting to download native
				//libs together with the wrapper repo mentioned above, compile libs and use the wrapper code for all your SDL needs
				//But guess what? That means I have to cross compile said library for all potential platforms and enjoy the headache
				//of ensuring that all bindings are properly working on target devices (and they may not, especially with shared libs),
				//the marshalling is not fucked up due to the fact that I use two different binding libs (Silk and SDL2-CS) and recompile
				//the libs with fresh changes from the original c lib repo every time the silk is updated
				//And here's a punchline: all of this mumbo jumbo, only to be able to use png textures in SDL2.
				//This seems like a lot of work for a sample project and something I'd rather leave to somebody else to do. For instance,
				//to Silk.NET creators. Otherwise I'd rather find a more simple way to load textures but that is not my priority atm
				//TODO: implement
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

				/*
				//Let's try this first
				SDL.RenderCopy(
					renderer,
					texture,
					null,
					null);

				SDL.RenderCopy(
					renderer,
					texture,
					new Rectangle<int>(
						0,
						0,
						640,
						480),
					new Rectangle<int>(
						0,
						0,
						640,
						480));
				*/

				// Switches out the currently presented render surface with the one we just did work on.
				SDL.RenderPresent(renderer);
			}

			/// <summary>
			/// Clean up the resources that were created.
			/// </summary>
			void CleanUp()
			{
				//SDL.DestroyTexture(texture);
				SDL.DestroyRenderer(renderer);
				SDL.DestroyWindow(window);
				SDL.Quit();
			}
		}
	}
}