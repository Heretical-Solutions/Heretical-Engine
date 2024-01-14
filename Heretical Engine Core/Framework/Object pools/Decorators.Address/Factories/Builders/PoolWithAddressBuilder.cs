using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public class PoolWithAddressBuilder<T>
    {
        private readonly ILoggerResolver loggerResolver;

        private readonly IFormatLogger logger;

        public PoolWithAddressBuilder(
            ILoggerResolver loggerResolver = null,
            IFormatLogger logger = null)
        {
            this.loggerResolver = loggerResolver;

            this.logger = logger;
        }

        /// <summary>
        /// The root node of the address tree.
        /// </summary>
        private AddressTreeNode<T> root;

        /// <summary>
        /// Initializes the pool builder.
        /// </summary>
        public void Initialize()
        {
            root = new AddressTreeNode<T>
            {
                CurrentAddress = string.Empty,

                FullAddress = string.Empty,

                CurrentAddressHash = -1,

                FullAddressHashes = new int[0],

                Level = 0,

                Pool = null
            };
        }

        /// <summary>
        /// Parses the address and sets the corresponding pool.
        /// </summary>
        /// <param name="address">The address to parse.</param>
        /// <param name="pool">The pool to set.</param>
        public void Parse(
            string address,
            INonAllocDecoratedPool<T> pool)
        {
            string[] addressParts = address.SplitAddressBySeparator();

            int[] addressHashes = address.AddressToHashes();

            AddressTreeNode<T> currentNode = root;

            for (int i = 0; i < addressHashes.Length; i++)
            {
                bool traversed = TraverseToChildNode(
                    ref currentNode,
                    addressHashes[i]);

                if (!traversed)
                    CreateAndTraverse(
                        addressParts,
                        addressHashes,
                        ref currentNode,
                        addressHashes[i]);
            }

            currentNode.Pool = pool;
        }

        /// <summary>
        /// Traverses to the child node with the target address hash.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="targetAddressHash">The target address hash.</param>
        /// <returns>True if the child node is found and traversed, false otherwise.</returns>
        private bool TraverseToChildNode(
            ref AddressTreeNode<T> currentNode,
            int targetAddressHash)
        {
            for (int i = 0; i < currentNode.Children.Count; i++)
            {
                if (currentNode.Children[i].CurrentAddressHash == targetAddressHash)
                {
                    currentNode = currentNode.Children[i];

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a child node and traverses to it.
        /// </summary>
        /// <param name="addressParts">The address parts.</param>
        /// <param name="addressHashes">The address hashes.</param>
        /// <param name="currentNode">The current node.</param>
        /// <param name="targetAddressHash">The target address hash.</param>
        private void CreateAndTraverse(
            string[] addressParts,
            int[] addressHashes,
            ref AddressTreeNode<T> currentNode,
            int targetAddressHash)
        {
            int currentNodeLevel = currentNode.Level;

            AddressTreeNode<T> child = new AddressTreeNode<T>
            {
                CurrentAddress = addressParts.AddressAtDepth(currentNodeLevel),

                //TODO: check if works correctly
                FullAddress = addressParts.PartialAddress(currentNodeLevel),

                CurrentAddressHash = targetAddressHash,

                FullAddressHashes = addressHashes.PartialAddressHashes(currentNodeLevel),

                Level = currentNodeLevel + 1,

                Pool = null
            };

            currentNode.Children.Add(child);

            currentNode = child;
        }

        /// <summary>
        /// Builds the pool with the address tree.
        /// </summary>
        /// <returns>The built pool.</returns>
        /// <exception cref="Exception">Thrown if the builder is not initialized.</exception>
        public INonAllocDecoratedPool<T> Build()
        {
            if (root == null)
                logger?.ThrowException<PoolWithAddressBuilder<T>>(
                    "BUILDER NOT INITIALIZED");

            var result = BuildPoolWithAddress(root);

            root = null;

            return result;
        }

        /// <summary>
        /// Recursively builds the pool with the address tree.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>The built pool.</returns>
        private INonAllocDecoratedPool<T> BuildPoolWithAddress(
            AddressTreeNode<T> node)
        {
            IRepository<int, INonAllocDecoratedPool<T>> repository =
                RepositoriesFactory.BuildDictionaryRepository<int, INonAllocDecoratedPool<T>>();

            foreach (var child in node.Children)
            {
                repository.Add(child.CurrentAddressHash, BuildPoolWithAddress(child));
            }

            if (node.Pool != null)
            {
                repository.Add(0, node.Pool);
            }

            return AddressDecoratorsPoolsFactory.BuildNonAllocPoolWithAddress(
                repository,
                node.Level,
                loggerResolver);
        }
    }
}