namespace HereticalSolutions.RandomGeneration
{
    /// <summary>
    /// Represents an interface for generating random numbers
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Generates a random floating-point number between the specified minimum and maximum values
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random floating-point number between the specified minimum and maximum values.</returns>
        float Random(float min, float max);
    }
}