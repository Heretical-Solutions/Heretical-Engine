using System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a pair of a GUID and a prototype ID.
    /// </summary>
    public struct GuidPrototypeIDPair
    {
        /// <summary>
        /// Gets or sets the GUID value.
        /// </summary>
        public Guid GUID;

        /// <summary>
        /// Gets or sets the prototype ID value.
        /// </summary>
        public string PrototypeID;
    }
}