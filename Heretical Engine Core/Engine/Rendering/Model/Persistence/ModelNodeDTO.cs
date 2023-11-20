// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using HereticalSolutions.HereticalEngine.Scenes;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	[Serializable]
	public struct ModelNodeDTO
	{
		public string Name;

		public ModelNodeDTO[] Children;

		public TransformDTO Transform;

		public string[] MeshResourcePaths;
	}
}