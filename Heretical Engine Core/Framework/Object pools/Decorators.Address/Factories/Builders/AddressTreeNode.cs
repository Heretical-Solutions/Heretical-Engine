using System.Collections.Generic;
 
namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a node in an address tree.
    /// </summary>
    /// <typeparam name="T">The type of objects in the node.</typeparam>
    public class AddressTreeNode<T>
    {
        /// <summary>
        /// Gets or sets the level of the node in the address tree.
        /// </summary>
        public int Level = 0;

        /// <summary>
        /// Gets or sets the list of children nodes.
        /// </summary>
        public List<AddressTreeNode<T>> Children = new List<AddressTreeNode<T>>();

        /// <summary>
        /// Gets or sets the current address of the node.
        /// </summary>
        public string CurrentAddress;

        /// <summary>
        /// Gets or sets the full address of the node.
        /// </summary>
        public string FullAddress;

        /// <summary>
        /// Gets or sets the hash code of the current address.
        /// </summary>
        public int CurrentAddressHash = -1;

        /// <summary>
        /// Gets or sets the array of hash codes for the full address.
        /// </summary>
        public int[] FullAddressHashes = null;

        /// <summary>
        /// Gets or sets the pool associated with the node.
        /// </summary>
        public INonAllocDecoratedPool<T> Pool;
    }
}