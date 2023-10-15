using System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Enumeration representing the authoring modes of an entity.
    /// </summary>
    public enum EEntityAuthoring
    {
        /// <summary>
        /// Offline authoring mode.
        /// </summary>
        OFFLINE,

        /// <summary>
        /// Networking host authoring mode.
        /// </summary>
        NETWORKING_HOST,

        /// <summary>
        /// Headless networking host authoring mode.
        /// </summary>
        NETWORKING_HOST_HEADLESS,

        /// <summary>
        /// Networking client authoring mode.
        /// </summary>
        NETWORKING_CLIENT
    }
}