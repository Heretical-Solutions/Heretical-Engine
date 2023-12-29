// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using HereticalSolutions.HereticalEngine.GameEntities;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	[Serializable]
	public struct ModelNodeDescriptor
	{
		public string Name;

		public ModelNodeDescriptor[] Children;

		public TransformDTO Transform;

		public string[] MeshResourcePaths;
	}
}