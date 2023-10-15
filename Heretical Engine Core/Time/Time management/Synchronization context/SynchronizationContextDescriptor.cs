using System;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents a synchronization context descriptor.
    /// </summary>
    public class SynchronizationContextDescriptor
    {
        /// <summary>
        /// Gets the ID of the synchronization context descriptor.
        /// </summary>
        public string ID { get; private set; }

        #region Toggleness

        /// <summary>
        /// Gets a value indicating whether the synchronization context descriptor can be toggled.
        /// </summary>
        public bool CanBeToggled { get; private set; }

        private bool active = true;

        /// <summary>
        /// Gets or sets a value indicating whether the synchronization context descriptor is active.
        /// </summary>
        /// <exception cref="Exception">Thrown when attempting to toggle an untoggable context.</exception>
        public bool Active
        {
            get { return active; }
            set
            {
                if (!CanBeToggled)
                    throw new Exception(
                        "[SynchronizationContextDescriptor] ATTEMPT TO TOGGLE UNTOGGLABLE CONTEXT");

                active = value;
            }
        }

        #endregion

        #region Scalability

        /// <summary>
        /// Gets a value indicating whether the synchronization context descriptor can scale.
        /// </summary>
        public bool CanScale { get; private set; }

        private float scale = 1f;

        /// <summary>
        /// Gets or sets the scale of the synchronization context descriptor.
        /// </summary>
        /// <exception cref="Exception">Thrown when attempting to change the scale of an unscaleable context.</exception>
        public float Scale
        {
            get { return scale; }
            set
            {
                if (!CanScale)
                    throw new Exception(
                        "[SynchronizationContextDescriptor] ATTEMPT TO CHANGE SCALE OF UNSCALABLE CONTEXT");

                scale = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextDescriptor"/> class.
        /// </summary>
        /// <param name="id">The ID of the synchronization context descriptor.</param>
        /// <param name="canBeToggled">A value indicating whether the synchronization context descriptor can be toggled.</param>
        /// <param name="canScale">A value indicating whether the synchronization context descriptor can scale.</param>
        public SynchronizationContextDescriptor(
            string id,
            bool canBeToggled,
            bool canScale)
        {
            ID = id;

            CanBeToggled = canBeToggled;

            CanScale = canScale;
        }
    }
}