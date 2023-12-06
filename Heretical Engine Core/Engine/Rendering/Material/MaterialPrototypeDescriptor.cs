// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace HereticalSolutions.HereticalEngine.Rendering
{
	[Serializable]
	public struct MaterialPrototypeDescriptor
	{
		public string Name;

		public string ShaderResourcePath;

		public string[] TextureResourcePaths;
	}
}