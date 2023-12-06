// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace HereticalSolutions.HereticalEngine.Rendering
{
	[Serializable]
	public struct TextureAssetDescriptor
	{
		public string Name;
		
		public string Type;


		public string WrapS;

		public string WrapT;

		public string MinFilter;

		public string MagFilter;


		public int BaseLevel;

		public int MaxLevel;
	}
}