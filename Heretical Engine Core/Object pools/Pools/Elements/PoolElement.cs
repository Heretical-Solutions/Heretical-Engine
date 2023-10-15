using HereticalSolutions.Repositories;

namespace HereticalSolutions.Pools.Elements
{
    /// <summary>
    /// Represents a pool element.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the element.</typeparam>
    public class PoolElement<T>
        : IPoolElement<T>,
          IPushable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PoolElement{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value for the element.</param>
        /// <param name="metadata">The metadata for the element.</param>
        public PoolElement(
            T defaultValue,
            IReadOnlyObjectRepository metadata)
        {
            Value = defaultValue;

            status = EPoolElementStatus.UNINITIALIZED;

            this.metadata = metadata;

            pushBehaviourHandler = null;
        }

        #region IPoolElement

        #region Value

        /// <summary>
        /// Gets or sets the value of the element.
        /// </summary>
        public T Value { get; set; }

        #endregion

        #region Status

        private EPoolElementStatus status;

        /// <summary>
        /// Gets or sets the status of the element.
        /// </summary>
        public EPoolElementStatus Status
        {
            get => status;
            set => status = value;
        }

        #endregion

        #region Metadata

        private readonly IReadOnlyObjectRepository metadata;

        /// <summary>
        /// Gets the metadata for the element.
        /// </summary>
        public IReadOnlyObjectRepository Metadata
        {
            get => metadata;
        }

        #endregion

        #region Push

        private IPushBehaviourHandler<T> pushBehaviourHandler;

        /// <summary>
        /// Pushes the element.
        /// </summary>
        public void Push()
        {
            if (status == EPoolElementStatus.PUSHED)
                return;

            pushBehaviourHandler?.Push(this);
        }

        #endregion

        #endregion

        #region IPushable

        /// <summary>
        /// Updates the push behaviour of the element.
        /// </summary>
        /// <param name="pushBehaviourHandler">The push behaviour handler.</param>
        public void UpdatePushBehaviour(IPushBehaviourHandler<T> pushBehaviourHandler)
        {
            this.pushBehaviourHandler = pushBehaviourHandler;
        }

        #endregion
    }
}