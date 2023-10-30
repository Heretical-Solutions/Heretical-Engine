// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshWithMaterialOpenGL
	{
		public MeshOpenGL MeshOpenGL { get; private set; }

		public MaterialOpenGL MaterialOpenGL { get; private set; }

		public MeshWithMaterialOpenGL(
			MeshOpenGL meshOpenGL,
			MaterialOpenGL materialOpenGL)
		{
			MeshOpenGL = meshOpenGL;

			MaterialOpenGL = materialOpenGL;
		}
	}
}