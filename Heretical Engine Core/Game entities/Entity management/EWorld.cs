namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents the different worlds in the game.
    /// </summary>
    public enum EWorld
    {
        /// <summary>
        /// The world used for registry operations.
        /// </summary>
        REGISTRY,

        /// <summary>
        /// The world used for prototype operations.
        /// </summary>
        PROTOTYPE,

        /// <summary>
        /// The world used for simulation operations.
        /// </summary>
        SIMULATION,

        /// <summary>
        /// The world used for server operations.
        /// </summary>
        SERVER,

        /// <summary>
        /// The world used for prediction operations.
        /// </summary>
        PREDICTION,

        /// <summary>
        /// The world used for event operations.
        /// </summary>
        EVENT,

        /// <summary>
        /// The world used for view operations.
        /// </summary>
        VIEW
    }
}