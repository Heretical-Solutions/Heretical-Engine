﻿using System;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using System.Drawing;

using Silk.NET.Core;
using Silk.NET.Core.Native;

using Silk.NET.Maths;

using Silk.NET.Windowing;

using Silk.NET.Input;

using Silk.NET.SDL;

//For Dear ImGUI
/*
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
*/

//For Vulkan
/*
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Semaphore = Silk.NET.Vulkan.Semaphore;
*/

namespace HereticalSolutions.HereticalEngine
{
	public class Program
	{
		static void Main(string[] args)
		{
		}

		//SDL. WIP as there is no SDL.something, need to adjust everything

		/*
		static void Main(string[] args)
		{
			new SDLSample().Run();
		}
		
		class SDLSample
		{
			IntPtr window;

			IntPtr renderer;

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
				// Initilizes SDL.
				if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
				{
					Console.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
				}

				// Create a new window given a title, size, and passes it a flag indicating it should be shown.
				window = SDL.SDL_CreateWindow(
					"SDL .NET 6 Tutorial",
					SDL.SDL_WINDOWPOS_UNDEFINED,
					SDL.SDL_WINDOWPOS_UNDEFINED,
					640,
					480,
					SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

				if (window == IntPtr.Zero)
				{
					Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
				}

				// Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
				renderer = SDL.SDL_CreateRenderer(
					window,
					-1,
					SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
					SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

				if (renderer == IntPtr.Zero)
				{
					Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
				}
			}

			/// <summary>
			/// Checks to see if there are any events to be processed.
			/// </summary>
			void PollEvents()
			{
				// Check to see if there are any events and continue to do so until the queue is empty.
				while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
				{
					switch (e.type)
					{
						case SDL.SDL_EventType.SDL_QUIT:
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
				SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255);

				// Clears the current render surface.
				SDL.SDL_RenderClear(renderer);

				// Switches out the currently presented render surface with the one we just did work on.
				SDL.SDL_RenderPresent(renderer);
			}

			/// <summary>
			/// Clean up the resources that were created.
			/// </summary>
			void CleanUp()
			{
				SDL.SDL_DestroyRenderer(renderer);
				SDL.SDL_DestroyWindow(window);
				SDL.SDL_Quit();
			}
		}
		*/

		//VULKAN TUTORIALS
		//https://github.com/dfkeenan/SilkVulkanTutorial/tree/main/Source
		//ORIGINAL:
		//https://github.com/Overv/VulkanTutorial
		//OFFICIAL:
		//https://vulkan-tutorial.com/

		/*
		//FIX 1
		//private readonly string[] validationLayers = new[]
		//{
		//		//"VK_LAYER_KHRONOS_validation" //DOES NOT WORK ON LINUX
		//		"VK_LAYER_LUNARG_standard_validation" //COURTESY OF https://stackoverflow.com/questions/73223160/when-i-follow-vulkan-tutorial-example-code-validation-layer-doesnt-work
		//};

		//FIX 2
		//.spv files are obtained from here
		//https://github.com/FacticiusVir/SharpVk-Samples/tree/master/SharpVk.HelloTriangle

		static void Main(string[] args)
		{
			var app = new HelloTriangleApplication();

			app.Run();
		}

		struct QueueFamilyIndices
		{
			public uint? GraphicsFamily { get; set; }
			public uint? PresentFamily { get; set; }

			public bool IsComplete()
			{
				return GraphicsFamily.HasValue && PresentFamily.HasValue;
			}
		}

		struct SwapChainSupportDetails
		{
			public SurfaceCapabilitiesKHR Capabilities;
			public SurfaceFormatKHR[] Formats;
			public PresentModeKHR[] PresentModes;
		}

		unsafe class HelloTriangleApplication
		{
			const int WIDTH = 800;
			const int HEIGHT = 600;

			const int MAX_FRAMES_IN_FLIGHT = 2;

			bool EnableValidationLayers = true;

			private readonly string[] validationLayers = new[]
			{
				//"VK_LAYER_KHRONOS_validation" //DOES NOT WORK ON LINUX
				"VK_LAYER_LUNARG_standard_validation" //COURTESY OF https://stackoverflow.com/questions/73223160/when-i-follow-vulkan-tutorial-example-code-validation-layer-doesnt-work
			};

			private readonly string[] deviceExtensions = new[]
			{
				KhrSwapchain.ExtensionName
			};

			private IWindow? window;
			private Vk? vk;

			private Instance instance;

			private ExtDebugUtils? debugUtils;
			private DebugUtilsMessengerEXT debugMessenger;
			private KhrSurface? khrSurface;
			private SurfaceKHR surface;

			private PhysicalDevice physicalDevice;
			private Device device;

			private Queue graphicsQueue;
			private Queue presentQueue;

			private KhrSwapchain? khrSwapChain;
			private SwapchainKHR swapChain;
			private Image[]? swapChainImages;
			private Format swapChainImageFormat;
			private Extent2D swapChainExtent;
			private ImageView[]? swapChainImageViews;
			private Framebuffer[]? swapChainFramebuffers;

			private RenderPass renderPass;
			private PipelineLayout pipelineLayout;
			private Pipeline graphicsPipeline;

			private CommandPool commandPool;
			private CommandBuffer[]? commandBuffers;

			private Semaphore[]? imageAvailableSemaphores;
			private Semaphore[]? renderFinishedSemaphores;
			private Fence[]? inFlightFences;
			private Fence[]? imagesInFlight;
			private int currentFrame = 0;

			public void Run()
			{
				InitWindow();
				InitVulkan();
				MainLoop();
				CleanUp();
			}

			private void InitWindow()
			{
				//Create a window.
				var options = WindowOptions.DefaultVulkan with
				{
					Size = new Vector2D<int>(WIDTH, HEIGHT),
					Title = "Vulkan",
				};

				window = Window.Create(options);
				window.Initialize();

				if (window.VkSurface is null)
				{
					throw new Exception("Windowing platform doesn't support Vulkan.");
				}
			}

			private void InitVulkan()
			{
				CreateInstance();
				SetupDebugMessenger();
				CreateSurface();
				PickPhysicalDevice();
				CreateLogicalDevice();
				CreateSwapChain();
				CreateImageViews();
				CreateRenderPass();
				CreateGraphicsPipeline();
				CreateFramebuffers();
				CreateCommandPool();
				CreateCommandBuffers();
				CreateSyncObjects();
			}

			private void MainLoop()
			{
				window!.Render += DrawFrame;
				window!.Run();
				vk!.DeviceWaitIdle(device);
			}

			private void CleanUp()
			{
				for (int i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
				{
					vk!.DestroySemaphore(device, renderFinishedSemaphores![i], null);
					vk!.DestroySemaphore(device, imageAvailableSemaphores![i], null);
					vk!.DestroyFence(device, inFlightFences![i], null);
				}

				vk!.DestroyCommandPool(device, commandPool, null);

				foreach (var framebuffer in swapChainFramebuffers!)
				{
					vk!.DestroyFramebuffer(device, framebuffer, null);
				}

				vk!.DestroyPipeline(device, graphicsPipeline, null);
				vk!.DestroyPipelineLayout(device, pipelineLayout, null);
				vk!.DestroyRenderPass(device, renderPass, null);

				foreach (var imageView in swapChainImageViews!)
				{
					vk!.DestroyImageView(device, imageView, null);
				}

				khrSwapChain!.DestroySwapchain(device, swapChain, null);

				vk!.DestroyDevice(device, null);

				if (EnableValidationLayers)
				{
					//DestroyDebugUtilsMessenger equivilant to method DestroyDebugUtilsMessengerEXT from original tutorial.
					debugUtils!.DestroyDebugUtilsMessenger(instance, debugMessenger, null);
				}

				khrSurface!.DestroySurface(instance, surface, null);
				vk!.DestroyInstance(instance, null);
				vk!.Dispose();

				window?.Dispose();
			}

			private void CreateInstance()
			{
				vk = Vk.GetApi();

				if (EnableValidationLayers && !CheckValidationLayerSupport())
				{
					throw new Exception("validation layers requested, but not available!");
				}

				ApplicationInfo appInfo = new()
				{
					SType = StructureType.ApplicationInfo,
					PApplicationName = (byte*)Marshal.StringToHGlobalAnsi("Hello Triangle"),
					ApplicationVersion = new Version32(1, 0, 0),
					PEngineName = (byte*)Marshal.StringToHGlobalAnsi("No Engine"),
					EngineVersion = new Version32(1, 0, 0),
					ApiVersion = Vk.Version12
				};

				InstanceCreateInfo createInfo = new()
				{
					SType = StructureType.InstanceCreateInfo,
					PApplicationInfo = &appInfo
				};

				var extensions = GetRequiredExtensions();
				createInfo.EnabledExtensionCount = (uint)extensions.Length;
				createInfo.PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(extensions); ;

				if (EnableValidationLayers)
				{
					createInfo.EnabledLayerCount = (uint)validationLayers.Length;
					createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(validationLayers);

					DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new();
					PopulateDebugMessengerCreateInfo(ref debugCreateInfo);
					createInfo.PNext = &debugCreateInfo;
				}
				else
				{
					createInfo.EnabledLayerCount = 0;
					createInfo.PNext = null;
				}

				if (vk.CreateInstance(createInfo, null, out instance) != Result.Success)
				{
					throw new Exception("failed to create instance!");
				}

				Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
				Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);
				SilkMarshal.Free((nint)createInfo.PpEnabledExtensionNames);

				if (EnableValidationLayers)
				{
					SilkMarshal.Free((nint)createInfo.PpEnabledLayerNames);
				}
			}

			private void PopulateDebugMessengerCreateInfo(ref DebugUtilsMessengerCreateInfoEXT createInfo)
			{
				createInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
				createInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
											 DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
											 DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt;
				createInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
										 DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
										 DebugUtilsMessageTypeFlagsEXT.ValidationBitExt;
				createInfo.PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)DebugCallback;
			}

			private void SetupDebugMessenger()
			{
				if (!EnableValidationLayers) return;

				//TryGetInstanceExtension equivilant to method CreateDebugUtilsMessengerEXT from original tutorial.
				if (!vk!.TryGetInstanceExtension(instance, out debugUtils)) return;

				DebugUtilsMessengerCreateInfoEXT createInfo = new();
				PopulateDebugMessengerCreateInfo(ref createInfo);

				if (debugUtils!.CreateDebugUtilsMessenger(instance, in createInfo, null, out debugMessenger) != Result.Success)
				{
					throw new Exception("failed to set up debug messenger!");
				}
			}

			private void CreateSurface()
			{
				if (!vk!.TryGetInstanceExtension<KhrSurface>(instance, out khrSurface))
				{
					throw new NotSupportedException("KHR_surface extension not found.");
				}

				surface = window!.VkSurface!.Create<AllocationCallbacks>(instance.ToHandle(), null).ToSurface();
			}

			private void PickPhysicalDevice()
			{
				uint devicedCount = 0;
				vk!.EnumeratePhysicalDevices(instance, ref devicedCount, null);

				if (devicedCount == 0)
				{
					throw new Exception("failed to find GPUs with Vulkan support!");
				}

				var devices = new PhysicalDevice[devicedCount];
				fixed (PhysicalDevice* devicesPtr = devices)
				{
					vk!.EnumeratePhysicalDevices(instance, ref devicedCount, devicesPtr);
				}

				foreach (var device in devices)
				{
					if (IsDeviceSuitable(device))
					{
						physicalDevice = device;
						break;
					}
				}

				if (physicalDevice.Handle == 0)
				{
					throw new Exception("failed to find a suitable GPU!");
				}
			}

			private void CreateLogicalDevice()
			{
				var indices = FindQueueFamilies(physicalDevice);

				var uniqueQueueFamilies = new[] { indices.GraphicsFamily!.Value, indices.PresentFamily!.Value };
				uniqueQueueFamilies = uniqueQueueFamilies.Distinct().ToArray();

				using var mem = GlobalMemory.Allocate(uniqueQueueFamilies.Length * sizeof(DeviceQueueCreateInfo));
				var queueCreateInfos = (DeviceQueueCreateInfo*)Unsafe.AsPointer(ref mem.GetPinnableReference());

				float queuePriority = 1.0f;
				for (int i = 0; i < uniqueQueueFamilies.Length; i++)
				{
					queueCreateInfos[i] = new()
					{
						SType = StructureType.DeviceQueueCreateInfo,
						QueueFamilyIndex = uniqueQueueFamilies[i],
						QueueCount = 1,
						PQueuePriorities = &queuePriority
					};
				}

				PhysicalDeviceFeatures deviceFeatures = new();

				DeviceCreateInfo createInfo = new()
				{
					SType = StructureType.DeviceCreateInfo,
					QueueCreateInfoCount = (uint)uniqueQueueFamilies.Length,
					PQueueCreateInfos = queueCreateInfos,

					PEnabledFeatures = &deviceFeatures,

					EnabledExtensionCount = (uint)deviceExtensions.Length,
					PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(deviceExtensions)
				};

				if (EnableValidationLayers)
				{
					createInfo.EnabledLayerCount = (uint)validationLayers.Length;
					createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(validationLayers);
				}
				else
				{
					createInfo.EnabledLayerCount = 0;
				}

				if (vk!.CreateDevice(physicalDevice, in createInfo, null, out device) != Result.Success)
				{
					throw new Exception("failed to create logical device!");
				}

				vk!.GetDeviceQueue(device, indices.GraphicsFamily!.Value, 0, out graphicsQueue);
				vk!.GetDeviceQueue(device, indices.PresentFamily!.Value, 0, out presentQueue);

				if (EnableValidationLayers)
				{
					SilkMarshal.Free((nint)createInfo.PpEnabledLayerNames);
				}

				SilkMarshal.Free((nint)createInfo.PpEnabledExtensionNames);

			}

			private void CreateSwapChain()
			{
				var swapChainSupport = QuerySwapChainSupport(physicalDevice);

				var surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
				var presentMode = ChoosePresentMode(swapChainSupport.PresentModes);
				var extent = ChooseSwapExtent(swapChainSupport.Capabilities);

				var imageCount = swapChainSupport.Capabilities.MinImageCount + 1;
				if (swapChainSupport.Capabilities.MaxImageCount > 0 && imageCount > swapChainSupport.Capabilities.MaxImageCount)
				{
					imageCount = swapChainSupport.Capabilities.MaxImageCount;
				}

				SwapchainCreateInfoKHR creatInfo = new()
				{
					SType = StructureType.SwapchainCreateInfoKhr,
					Surface = surface,

					MinImageCount = imageCount,
					ImageFormat = surfaceFormat.Format,
					ImageColorSpace = surfaceFormat.ColorSpace,
					ImageExtent = extent,
					ImageArrayLayers = 1,
					ImageUsage = ImageUsageFlags.ColorAttachmentBit,
				};

				var indices = FindQueueFamilies(physicalDevice);
				var queueFamilyIndices = stackalloc[] { indices.GraphicsFamily!.Value, indices.PresentFamily!.Value };

				if (indices.GraphicsFamily != indices.PresentFamily)
				{
					creatInfo = creatInfo with
					{
						ImageSharingMode = SharingMode.Concurrent,
						QueueFamilyIndexCount = 2,
						PQueueFamilyIndices = queueFamilyIndices,
					};
				}
				else
				{
					creatInfo.ImageSharingMode = SharingMode.Exclusive;
				}

				creatInfo = creatInfo with
				{
					PreTransform = swapChainSupport.Capabilities.CurrentTransform,
					CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
					PresentMode = presentMode,
					Clipped = true,

					OldSwapchain = default
				};

				if (!vk!.TryGetDeviceExtension(instance, device, out khrSwapChain))
				{
					throw new NotSupportedException("VK_KHR_swapchain extension not found.");
				}

				if (khrSwapChain!.CreateSwapchain(device, creatInfo, null, out swapChain) != Result.Success)
				{
					throw new Exception("failed to create swap chain!");
				}

				khrSwapChain.GetSwapchainImages(device, swapChain, ref imageCount, null);
				swapChainImages = new Image[imageCount];
				fixed (Image* swapChainImagesPtr = swapChainImages)
				{
					khrSwapChain.GetSwapchainImages(device, swapChain, ref imageCount, swapChainImagesPtr);
				}

				swapChainImageFormat = surfaceFormat.Format;
				swapChainExtent = extent;
			}

			private void CreateImageViews()
			{
				swapChainImageViews = new ImageView[swapChainImages!.Length];

				for (int i = 0; i < swapChainImages.Length; i++)
				{
					ImageViewCreateInfo createInfo = new()
					{
						SType = StructureType.ImageViewCreateInfo,
						Image = swapChainImages[i],
						ViewType = ImageViewType.Type2D,
						Format = swapChainImageFormat,
						Components =
				{
					R = ComponentSwizzle.Identity,
					G = ComponentSwizzle.Identity,
					B = ComponentSwizzle.Identity,
					A = ComponentSwizzle.Identity,
				},
						SubresourceRange =
				{
					AspectMask = ImageAspectFlags.ColorBit,
					BaseMipLevel = 0,
					LevelCount = 1,
					BaseArrayLayer = 0,
					LayerCount = 1,
				}

					};

					if (vk!.CreateImageView(device, createInfo, null, out swapChainImageViews[i]) != Result.Success)
					{
						throw new Exception("failed to create image views!");
					}
				}
			}

			private void CreateRenderPass()
			{
				AttachmentDescription colorAttachment = new()
				{
					Format = swapChainImageFormat,
					Samples = SampleCountFlags.Count1Bit,
					LoadOp = AttachmentLoadOp.Clear,
					StoreOp = AttachmentStoreOp.Store,
					StencilLoadOp = AttachmentLoadOp.DontCare,
					InitialLayout = ImageLayout.Undefined,
					FinalLayout = ImageLayout.PresentSrcKhr,
				};

				AttachmentReference colorAttachmentRef = new()
				{
					Attachment = 0,
					Layout = ImageLayout.ColorAttachmentOptimal,
				};

				SubpassDescription subpass = new()
				{
					PipelineBindPoint = PipelineBindPoint.Graphics,
					ColorAttachmentCount = 1,
					PColorAttachments = &colorAttachmentRef,
				};

				SubpassDependency dependency = new()
				{
					SrcSubpass = Vk.SubpassExternal,
					DstSubpass = 0,
					SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit,
					SrcAccessMask = 0,
					DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit,
					DstAccessMask = AccessFlags.ColorAttachmentWriteBit
				};

				RenderPassCreateInfo renderPassInfo = new()
				{
					SType = StructureType.RenderPassCreateInfo,
					AttachmentCount = 1,
					PAttachments = &colorAttachment,
					SubpassCount = 1,
					PSubpasses = &subpass,
					DependencyCount = 1,
					PDependencies = &dependency,
				};

				if (vk!.CreateRenderPass(device, renderPassInfo, null, out renderPass) != Result.Success)
				{
					throw new Exception("failed to create render pass!");
				}
			}

			private void CreateGraphicsPipeline()
			{
				var vertShaderCode = File.ReadAllBytes("shaders/vert.spv");
				var fragShaderCode = File.ReadAllBytes("shaders/frag.spv");

				var vertShaderModule = CreateShaderModule(vertShaderCode);
				var fragShaderModule = CreateShaderModule(fragShaderCode);

				PipelineShaderStageCreateInfo vertShaderStageInfo = new()
				{
					SType = StructureType.PipelineShaderStageCreateInfo,
					Stage = ShaderStageFlags.VertexBit,
					Module = vertShaderModule,
					PName = (byte*)SilkMarshal.StringToPtr("main")
				};

				PipelineShaderStageCreateInfo fragShaderStageInfo = new()
				{
					SType = StructureType.PipelineShaderStageCreateInfo,
					Stage = ShaderStageFlags.FragmentBit,
					Module = fragShaderModule,
					PName = (byte*)SilkMarshal.StringToPtr("main")
				};

				var shaderStages = stackalloc[]
				{
			vertShaderStageInfo,
			fragShaderStageInfo
		};

				PipelineVertexInputStateCreateInfo vertexInputInfo = new()
				{
					SType = StructureType.PipelineVertexInputStateCreateInfo,
					VertexBindingDescriptionCount = 0,
					VertexAttributeDescriptionCount = 0,
				};

				PipelineInputAssemblyStateCreateInfo inputAssembly = new()
				{
					SType = StructureType.PipelineInputAssemblyStateCreateInfo,
					Topology = PrimitiveTopology.TriangleList,
					PrimitiveRestartEnable = false,
				};

				Viewport viewport = new()
				{
					X = 0,
					Y = 0,
					Width = swapChainExtent.Width,
					Height = swapChainExtent.Height,
					MinDepth = 0,
					MaxDepth = 1,
				};

				Rect2D scissor = new()
				{
					Offset = { X = 0, Y = 0 },
					Extent = swapChainExtent,
				};

				PipelineViewportStateCreateInfo viewportState = new()
				{
					SType = StructureType.PipelineViewportStateCreateInfo,
					ViewportCount = 1,
					PViewports = &viewport,
					ScissorCount = 1,
					PScissors = &scissor,
				};

				PipelineRasterizationStateCreateInfo rasterizer = new()
				{
					SType = StructureType.PipelineRasterizationStateCreateInfo,
					DepthClampEnable = false,
					RasterizerDiscardEnable = false,
					PolygonMode = PolygonMode.Fill,
					LineWidth = 1,
					CullMode = CullModeFlags.BackBit,
					FrontFace = FrontFace.Clockwise,
					DepthBiasEnable = false,
				};

				PipelineMultisampleStateCreateInfo multisampling = new()
				{
					SType = StructureType.PipelineMultisampleStateCreateInfo,
					SampleShadingEnable = false,
					RasterizationSamples = SampleCountFlags.Count1Bit,
				};

				PipelineColorBlendAttachmentState colorBlendAttachment = new()
				{
					ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,
					BlendEnable = false,
				};

				PipelineColorBlendStateCreateInfo colorBlending = new()
				{
					SType = StructureType.PipelineColorBlendStateCreateInfo,
					LogicOpEnable = false,
					LogicOp = LogicOp.Copy,
					AttachmentCount = 1,
					PAttachments = &colorBlendAttachment,
				};

				colorBlending.BlendConstants[0] = 0;
				colorBlending.BlendConstants[1] = 0;
				colorBlending.BlendConstants[2] = 0;
				colorBlending.BlendConstants[3] = 0;

				PipelineLayoutCreateInfo pipelineLayoutInfo = new()
				{
					SType = StructureType.PipelineLayoutCreateInfo,
					SetLayoutCount = 0,
					PushConstantRangeCount = 0,
				};

				if (vk!.CreatePipelineLayout(device, pipelineLayoutInfo, null, out pipelineLayout) != Result.Success)
				{
					throw new Exception("failed to create pipeline layout!");
				}

				GraphicsPipelineCreateInfo pipelineInfo = new()
				{
					SType = StructureType.GraphicsPipelineCreateInfo,
					StageCount = 2,
					PStages = shaderStages,
					PVertexInputState = &vertexInputInfo,
					PInputAssemblyState = &inputAssembly,
					PViewportState = &viewportState,
					PRasterizationState = &rasterizer,
					PMultisampleState = &multisampling,
					PColorBlendState = &colorBlending,
					Layout = pipelineLayout,
					RenderPass = renderPass,
					Subpass = 0,
					BasePipelineHandle = default
				};

				if (vk!.CreateGraphicsPipelines(device, default, 1, pipelineInfo, null, out graphicsPipeline) != Result.Success)
				{
					throw new Exception("failed to create graphics pipeline!");
				}


				vk!.DestroyShaderModule(device, fragShaderModule, null);
				vk!.DestroyShaderModule(device, vertShaderModule, null);

				SilkMarshal.Free((nint)vertShaderStageInfo.PName);
				SilkMarshal.Free((nint)fragShaderStageInfo.PName);
			}

			private void CreateFramebuffers()
			{
				swapChainFramebuffers = new Framebuffer[swapChainImageViews!.Length];

				for (int i = 0; i < swapChainImageViews.Length; i++)
				{
					var attachment = swapChainImageViews[i];

					FramebufferCreateInfo framebufferInfo = new()
					{
						SType = StructureType.FramebufferCreateInfo,
						RenderPass = renderPass,
						AttachmentCount = 1,
						PAttachments = &attachment,
						Width = swapChainExtent.Width,
						Height = swapChainExtent.Height,
						Layers = 1,
					};

					if (vk!.CreateFramebuffer(device, framebufferInfo, null, out swapChainFramebuffers[i]) != Result.Success)
					{
						throw new Exception("failed to create framebuffer!");
					}
				}
			}

			private void CreateCommandPool()
			{
				var queueFamiliyIndicies = FindQueueFamilies(physicalDevice);

				CommandPoolCreateInfo poolInfo = new()
				{
					SType = StructureType.CommandPoolCreateInfo,
					QueueFamilyIndex = queueFamiliyIndicies.GraphicsFamily!.Value,
				};

				if (vk!.CreateCommandPool(device, poolInfo, null, out commandPool) != Result.Success)
				{
					throw new Exception("failed to create command pool!");
				}
			}

			private void CreateCommandBuffers()
			{
				commandBuffers = new CommandBuffer[swapChainFramebuffers!.Length];

				CommandBufferAllocateInfo allocInfo = new()
				{
					SType = StructureType.CommandBufferAllocateInfo,
					CommandPool = commandPool,
					Level = CommandBufferLevel.Primary,
					CommandBufferCount = (uint)commandBuffers.Length,
				};

				fixed (CommandBuffer* commandBuffersPtr = commandBuffers)
				{
					if (vk!.AllocateCommandBuffers(device, allocInfo, commandBuffersPtr) != Result.Success)
					{
						throw new Exception("failed to allocate command buffers!");
					}
				}


				for (int i = 0; i < commandBuffers.Length; i++)
				{
					CommandBufferBeginInfo beginInfo = new()
					{
						SType = StructureType.CommandBufferBeginInfo,
					};

					if (vk!.BeginCommandBuffer(commandBuffers[i], beginInfo) != Result.Success)
					{
						throw new Exception("failed to begin recording command buffer!");
					}

					RenderPassBeginInfo renderPassInfo = new()
					{
						SType = StructureType.RenderPassBeginInfo,
						RenderPass = renderPass,
						Framebuffer = swapChainFramebuffers[i],
						RenderArea =
				{
					Offset = { X = 0, Y = 0 },
					Extent = swapChainExtent,
				}
					};

					ClearValue clearColor = new()
					{
						Color = new() { Float32_0 = 0, Float32_1 = 0, Float32_2 = 0, Float32_3 = 1 },
					};

					renderPassInfo.ClearValueCount = 1;
					renderPassInfo.PClearValues = &clearColor;

					vk!.CmdBeginRenderPass(commandBuffers[i], &renderPassInfo, SubpassContents.Inline);

					vk!.CmdBindPipeline(commandBuffers[i], PipelineBindPoint.Graphics, graphicsPipeline);

					vk!.CmdDraw(commandBuffers[i], 3, 1, 0, 0);

					vk!.CmdEndRenderPass(commandBuffers[i]);

					if (vk!.EndCommandBuffer(commandBuffers[i]) != Result.Success)
					{
						throw new Exception("failed to record command buffer!");
					}

				}
			}

			private void CreateSyncObjects()
			{
				imageAvailableSemaphores = new Semaphore[MAX_FRAMES_IN_FLIGHT];
				renderFinishedSemaphores = new Semaphore[MAX_FRAMES_IN_FLIGHT];
				inFlightFences = new Fence[MAX_FRAMES_IN_FLIGHT];
				imagesInFlight = new Fence[swapChainImages!.Length];

				SemaphoreCreateInfo semaphoreInfo = new()
				{
					SType = StructureType.SemaphoreCreateInfo,
				};

				FenceCreateInfo fenceInfo = new()
				{
					SType = StructureType.FenceCreateInfo,
					Flags = FenceCreateFlags.SignaledBit,
				};

				for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
				{
					if (vk!.CreateSemaphore(device, semaphoreInfo, null, out imageAvailableSemaphores[i]) != Result.Success ||
						vk!.CreateSemaphore(device, semaphoreInfo, null, out renderFinishedSemaphores[i]) != Result.Success ||
						vk!.CreateFence(device, fenceInfo, null, out inFlightFences[i]) != Result.Success)
					{
						throw new Exception("failed to create synchronization objects for a frame!");
					}
				}
			}

			private void DrawFrame(double delta)
			{
				vk!.WaitForFences(device, 1, inFlightFences![currentFrame], true, ulong.MaxValue);

				uint imageIndex = 0;
				khrSwapChain!.AcquireNextImage(device, swapChain, ulong.MaxValue, imageAvailableSemaphores![currentFrame], default, ref imageIndex);

				if (imagesInFlight![imageIndex].Handle != default)
				{
					vk!.WaitForFences(device, 1, imagesInFlight[imageIndex], true, ulong.MaxValue);
				}
				imagesInFlight[imageIndex] = inFlightFences[currentFrame];

				SubmitInfo submitInfo = new()
				{
					SType = StructureType.SubmitInfo,
				};

				var waitSemaphores = stackalloc[] { imageAvailableSemaphores[currentFrame] };
				var waitStages = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };

				var buffer = commandBuffers![imageIndex];

				submitInfo = submitInfo with
				{
					WaitSemaphoreCount = 1,
					PWaitSemaphores = waitSemaphores,
					PWaitDstStageMask = waitStages,

					CommandBufferCount = 1,
					PCommandBuffers = &buffer
				};

				var signalSemaphores = stackalloc[] { renderFinishedSemaphores![currentFrame] };
				submitInfo = submitInfo with
				{
					SignalSemaphoreCount = 1,
					PSignalSemaphores = signalSemaphores,
				};

				vk!.ResetFences(device, 1, inFlightFences[currentFrame]);

				if (vk!.QueueSubmit(graphicsQueue, 1, submitInfo, inFlightFences[currentFrame]) != Result.Success)
				{
					throw new Exception("failed to submit draw command buffer!");
				}

				var swapChains = stackalloc[] { swapChain };
				PresentInfoKHR presentInfo = new()
				{
					SType = StructureType.PresentInfoKhr,

					WaitSemaphoreCount = 1,
					PWaitSemaphores = signalSemaphores,

					SwapchainCount = 1,
					PSwapchains = swapChains,

					PImageIndices = &imageIndex
				};

				khrSwapChain.QueuePresent(presentQueue, presentInfo);

				currentFrame = (currentFrame + 1) % MAX_FRAMES_IN_FLIGHT;

			}

			private ShaderModule CreateShaderModule(byte[] code)
			{
				ShaderModuleCreateInfo createInfo = new()
				{
					SType = StructureType.ShaderModuleCreateInfo,
					CodeSize = (nuint)code.Length,
				};

				ShaderModule shaderModule;

				fixed (byte* codePtr = code)
				{
					createInfo.PCode = (uint*)codePtr;

					if (vk!.CreateShaderModule(device, createInfo, null, out shaderModule) != Result.Success)
					{
						throw new Exception();
					}
				}

				return shaderModule;

			}

			private SurfaceFormatKHR ChooseSwapSurfaceFormat(IReadOnlyList<SurfaceFormatKHR> availableFormats)
			{
				foreach (var availableFormat in availableFormats)
				{
					if (availableFormat.Format == Format.B8G8R8A8Srgb && availableFormat.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
					{
						return availableFormat;
					}
				}

				return availableFormats[0];
			}

			private PresentModeKHR ChoosePresentMode(IReadOnlyList<PresentModeKHR> availablePresentModes)
			{
				foreach (var availablePresentMode in availablePresentModes)
				{
					if (availablePresentMode == PresentModeKHR.MailboxKhr)
					{
						return availablePresentMode;
					}
				}

				return PresentModeKHR.FifoKhr;
			}

			private Extent2D ChooseSwapExtent(SurfaceCapabilitiesKHR capabilities)
			{
				if (capabilities.CurrentExtent.Width != uint.MaxValue)
				{
					return capabilities.CurrentExtent;
				}
				else
				{
					var framebufferSize = window!.FramebufferSize;

					Extent2D actualExtent = new()
					{
						Width = (uint)framebufferSize.X,
						Height = (uint)framebufferSize.Y
					};

					actualExtent.Width = Math.Clamp(actualExtent.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
					actualExtent.Height = Math.Clamp(actualExtent.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);

					return actualExtent;
				}
			}

			private SwapChainSupportDetails QuerySwapChainSupport(PhysicalDevice physicalDevice)
			{
				var details = new SwapChainSupportDetails();

				khrSurface!.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, out details.Capabilities);

				uint formatCount = 0;
				khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, ref formatCount, null);

				if (formatCount != 0)
				{
					details.Formats = new SurfaceFormatKHR[formatCount];
					fixed (SurfaceFormatKHR* formatsPtr = details.Formats)
					{
						khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, ref formatCount, formatsPtr);
					}
				}
				else
				{
					details.Formats = Array.Empty<SurfaceFormatKHR>();
				}

				uint presentModeCount = 0;
				khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, ref presentModeCount, null);

				if (presentModeCount != 0)
				{
					details.PresentModes = new PresentModeKHR[presentModeCount];
					fixed (PresentModeKHR* formatsPtr = details.PresentModes)
					{
						khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, ref presentModeCount, formatsPtr);
					}

				}
				else
				{
					details.PresentModes = Array.Empty<PresentModeKHR>();
				}

				return details;
			}

			private bool IsDeviceSuitable(PhysicalDevice device)
			{
				var indices = FindQueueFamilies(device);

				bool extensionsSupported = CheckDeviceExtensionsSupport(device);

				bool swapChainAdequate = false;
				if (extensionsSupported)
				{
					var swapChainSupport = QuerySwapChainSupport(device);
					swapChainAdequate = swapChainSupport.Formats.Any() && swapChainSupport.PresentModes.Any();
				}

				return indices.IsComplete() && extensionsSupported && swapChainAdequate;
			}

			private bool CheckDeviceExtensionsSupport(PhysicalDevice device)
			{
				uint extentionsCount = 0;
				vk!.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, null);

				var availableExtensions = new ExtensionProperties[extentionsCount];
				fixed (ExtensionProperties* availableExtensionsPtr = availableExtensions)
				{
					vk!.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, availableExtensionsPtr);
				}

				var availableExtensionNames = availableExtensions.Select(extension => Marshal.PtrToStringAnsi((IntPtr)extension.ExtensionName)).ToHashSet();

				return deviceExtensions.All(availableExtensionNames.Contains);

			}

			private QueueFamilyIndices FindQueueFamilies(PhysicalDevice device)
			{
				var indices = new QueueFamilyIndices();

				uint queueFamilityCount = 0;
				vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, null);

				var queueFamilies = new QueueFamilyProperties[queueFamilityCount];
				fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies)
				{
					vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, queueFamiliesPtr);
				}


				uint i = 0;
				foreach (var queueFamily in queueFamilies)
				{
					if (queueFamily.QueueFlags.HasFlag(QueueFlags.GraphicsBit))
					{
						indices.GraphicsFamily = i;
					}

					khrSurface!.GetPhysicalDeviceSurfaceSupport(device, i, surface, out var presentSupport);

					if (presentSupport)
					{
						indices.PresentFamily = i;
					}

					if (indices.IsComplete())
					{
						break;
					}

					i++;
				}

				return indices;
			}

			private string[] GetRequiredExtensions()
			{
				var glfwExtensions = window!.VkSurface!.GetRequiredExtensions(out var glfwExtensionCount);
				var extensions = SilkMarshal.PtrToStringArray((nint)glfwExtensions, (int)glfwExtensionCount);

				if (EnableValidationLayers)
				{
					return extensions.Append(ExtDebugUtils.ExtensionName).ToArray();
				}

				return extensions;
			}

			private bool CheckValidationLayerSupport()
			{
				uint layerCount = 0;
				vk!.EnumerateInstanceLayerProperties(ref layerCount, null);
				var availableLayers = new LayerProperties[layerCount];
				fixed (LayerProperties* availableLayersPtr = availableLayers)
				{
					vk!.EnumerateInstanceLayerProperties(ref layerCount, availableLayersPtr);
				}

				var availableLayerNames = availableLayers.Select(layer => Marshal.PtrToStringAnsi((IntPtr)layer.LayerName)).ToHashSet();

				return validationLayers.All(availableLayerNames.Contains);
			}

			private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
			{
				Console.WriteLine($"validation layer:" + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

				return Vk.False;
			}
		}
		*/

		//Silk.NET TUTORIALS:
		//https://github.com/dotnet/Silk.NET/tree/main/examples/CSharp

		//Dear IMGUI tutorial
		/*
		static void Main(string[] args)
		{
			// Create a Silk.NET window as usual
			using var window = Window.Create(WindowOptions.Default);

			// Declare some variables
			ImGuiController controller = null;
			GL gl = null;
			IInputContext inputContext = null;

			// Our loading function
			window.Load += () =>
			{
				controller = new ImGuiController(
					gl = window.CreateOpenGL(), // load OpenGL
					window, // pass in our window
					inputContext = window.CreateInput() // create an input context
				);
			};

			// Handle resizes
			window.FramebufferResize += s =>
			{
				// Adjust the viewport to the new window size
				gl.Viewport(s);
			};

			// The render function
			window.Render += delta =>
			{
				// Make sure ImGui is up-to-date
				controller.Update((float)delta);

				// This is where you'll do any rendering beneath the ImGui context
				// Here, we just have a blank screen.
				gl.ClearColor(Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));
				gl.Clear((uint)ClearBufferMask.ColorBufferBit);

				// This is where you'll do all of your ImGUi rendering
				// Here, we're just showing the ImGui built-in demo window.
				ImGuiNET.ImGui.ShowDemoWindow();

				// Make sure ImGui renders too!
				controller.Render();
			};

			// The closing function
			window.Closing += () =>
			{
				// Dispose our controller first
				controller?.Dispose();

				// Dispose the input context
				inputContext?.Dispose();

				// Unload OpenGL
				gl?.Dispose();
			};

			// Now that everything's defined, let's run this bad boy!
			window.Run();

			window.Dispose();
		}
		*/

		//Simple window tutorial
		/*
		private static IWindow window;

		static void Main(string[] args)
		{
			//Create a window.
			var options = WindowOptions.Default;
			options.Size = new Vector2D<int>(800, 600);
			options.Title = "LearnOpenGL with Silk.NET";

			window = Window.Create(options);

			//Assign events.
			window.Load += OnLoad;
			window.Update += OnUpdate;
			window.Render += OnRender;

			//Run the window.
			window.Run();

			// window.Run() is a BLOCKING method - this means that it will halt execution of any code in the current
			// method until the window has finished running. Therefore, this dispose method will not be called until you
			// close the window.
			window.Dispose();
		}

		private static void OnLoad()
		{
			//Set-up input context.
			IInputContext input = window.CreateInput();
			for (int i = 0; i < input.Keyboards.Count; i++)
			{
				input.Keyboards[i].KeyDown += KeyDown;
			}
		}

		private static void OnRender(double obj)
		{
			//Here all rendering should be done.
		}

		private static void OnUpdate(double obj)
		{
			//Here all updates to the program should be done.
		}

		private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
		{
			//Check to close the window on escape.
			if (arg2 == Key.Escape)
			{
				window.Close();
			}
		}
		*/
	}
}