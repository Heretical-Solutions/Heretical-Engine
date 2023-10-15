using System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a data transfer object for an ECS component.
    /// </summary>
    [Serializable]
    public struct ECSComponentDTO
    {
        /// <summary>
        /// Gets or sets the hash code representing the type of the component.
        /// </summary>
        /// <value>
        /// The hash code representing the type of the component.
        /// </value>
        public int TypeHash;

        /// <summary>
        /// Gets or sets the data of the component.
        /// </summary>
        /// <value>
        /// The data of the component.
        /// </value>
        public byte[] Data;
    }
}