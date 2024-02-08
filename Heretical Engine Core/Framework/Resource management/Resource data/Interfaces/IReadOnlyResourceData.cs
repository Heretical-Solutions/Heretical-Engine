namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents read-only resource data
    /// </summary>
    public interface IReadOnlyResourceData
        : IContainsResourceVariants,
          IContainsNestedResources
    {
        /// <summary>
        /// Gets the descriptor of the resource
        /// </summary>
        ResourceDescriptor Descriptor { get; }
    }
}