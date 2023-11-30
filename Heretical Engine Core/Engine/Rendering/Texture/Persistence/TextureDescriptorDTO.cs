// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	[Serializable]
	public struct TextureDescriptorDTO
	{
		public string Name;
		
		public TextureType Type;

		public int WrapS;

		public int WrapT;

		public int MinFilter;

		public int MagFilter;

		public int BaseLevel;

		public int MaxLevel;
	}
}