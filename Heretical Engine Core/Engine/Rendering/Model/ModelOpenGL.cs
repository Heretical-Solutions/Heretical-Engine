// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelOpenGL
	{
		public MeshOpenGL[] Meshes { get; private set; }

		public TextureOpenGL[] Textures { get; private set; }

		public MaterialOpenGL[] Materials { get; private set; }

		public ModelNodeOpenGL RootNode { get; private set; }

		public ModelOpenGL(
			MeshOpenGL[] meshes,
			TextureOpenGL[] textures,
			MaterialOpenGL[] materials,
			ModelNodeOpenGL rootNode)
		{
			Meshes = meshes;

			Textures = textures;

			Materials = materials;
			
			RootNode = rootNode;
		}
	}
}