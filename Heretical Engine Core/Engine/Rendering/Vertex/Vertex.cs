// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//using System.Numerics;
using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct Vertex
	{
		public Vector3D<float> Position;

		public Vector3D<float> Normal;

		public Vector3D<float> Tangent;

		public Vector3D<float> Bitangent;

		public Vector2D<float> UV0;

		public Vector2D<float> UV1;

		public Vector2D<float> UV2;

		public Vector2D<float> UV3;

		public Vector4D<float> Color;


		public const int MAX_BONE_INFLUENCE = 4;

		/*
		public int[] BoneIds;
		
		public float[] Weights;
		*/
	}
}