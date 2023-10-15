namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a descriptor for a resource.
    /// </summary>
    public struct ResourceDescriptor
    {
        /// <summary>
        /// The ID of the resource.
        /// </summary>
        public string ID;

        /// <summary>
        /// The hash value of the resource ID.
        /// </summary>
        public int IDHash;
    }
}