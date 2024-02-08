namespace HereticalSolutions.RandomGeneration.Factories
{
    /// <summary>
    /// Represents a factory class for creating random generators
    /// </summary>
    public static class RandomFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SystemRandomGenerator"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="SystemRandomGenerator"/> class.</returns>
        public static SystemRandomGenerator BuildSystemRandomGenerator()
        {
            return new SystemRandomGenerator();
        }
    }
}