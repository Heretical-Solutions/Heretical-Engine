using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderAssimp : AssetImporter
	{
		private string resourceID;

		private ISerializer serializer;

		private ISerializationArgument vertexShaderSerializationArgument;

		private ISerializationArgument fragmentShaderSerializationArgument;

		private ILoadVisitorGeneric<Shader, VertexFragmentShaderDTO> visitor;

		public ShaderAssimp(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			ISerializer serializer,
			ISerializationArgument vertexShaderSerializationArgument,
			ISerializationArgument fragmentShaderSerializationArgument,
			ILoadVisitorGeneric<Shader, VertexFragmentShaderDTO> visitor)
			: base(
				resourceManager)
		{
			this.resourceID = resourceID;

			this.serializer = serializer;

			this.vertexShaderSerializationArgument = vertexShaderSerializationArgument;

			this.fragmentShaderSerializationArgument = fragmentShaderSerializationArgument;

			this.visitor = visitor;
		}

		public override void Import()
		{
			VertexFragmentShaderDTO dto = new VertexFragmentShaderDTO();

			serializer.Deserialize<ShaderSourceDTO>(
				vertexShaderSerializationArgument,
				out dto.Vertex);

			serializer.Deserialize<ShaderSourceDTO>(
				fragmentShaderSerializationArgument,
				out dto.Fragment);

			visitor.Load(
				dto,
				out var asset);

			IResourceData resourceData = GetResourceData(resourceID);

			AddResource(
				asset,
				resourceData);
		}

		protected IResourceData GetResourceData(
			string resourceID)
		{
			IResourceData resourceData = null;

			if (resourceManager.HasResource(resourceID))
			{
				resourceData = (IResourceData)resourceManager.GetResource(resourceID);
			}
			else
			{
				resourceData = RuntimeResourceManagerFactory.BuildResourceData(
					new ResourceDescriptor()
					{
						ID = resourceID,
						IDHash = resourceID.AddressToHash()
					});

				resourceManager.AddResource((IReadOnlyResourceData)resourceData);
			}

			return resourceData;
		}

		private void AddResource(
			Shader asset,
			IResourceData resourceData,
			IProgress<float> progress = null)
		{
			AddResourceAsDefault(
				asset,
				resourceData,
				progress);
		}

		private void AddResourceAsDefault(
			Shader asset,
			IResourceData resourceData,
			IProgress<float> progress = null)
		{
			var variantData = RuntimeResourceManagerFactory.BuildResourceVariantData(
					new ResourceVariantDescriptor()
					{
						VariantID = string.Empty,
						VariantIDHash = string.Empty.AddressToHash(),
						Priority = 0,
						Source = EResourceSources.LOCAL_STORAGE,
						ResourceType = typeof(Shader)
					},
					RuntimeResourceManagerFactory.BuildRuntimeResourceStorageHandle(
						asset));

			resourceData.AddVariant(
				variantData,
				progress);
		}
	}
}