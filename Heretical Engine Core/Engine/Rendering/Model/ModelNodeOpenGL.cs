// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using HereticalSolutions.HereticalEngine.Scenes;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelNodeOpenGL
	{
		public string Name { get; private set; }

		public ModelNodeOpenGL Parent { get; private set; }

		public ModelNodeOpenGL[] Children { get; private set; }

		public Transform Transform { get; private set; }

		public MeshWithMaterialOpenGL[] MeshesWithMaterials { get; private set; }

		public ModelNodeOpenGL(
			string name,
			ModelNodeOpenGL parent,
			ModelNodeOpenGL[] children,
			Transform transform,
			MeshWithMaterialOpenGL[] meshesWithMaterials)
		{
			Name = name;

			Parent = parent;

			Children = children;

			Transform = transform;

			MeshesWithMaterials = meshesWithMaterials;
		}
	}
}