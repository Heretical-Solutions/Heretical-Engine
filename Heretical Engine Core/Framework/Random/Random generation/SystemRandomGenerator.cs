using System;

namespace HereticalSolutions.RandomGeneration
{
    /// <summary>
    /// A random number generator that uses the System.Random class
    /// </summary>
    public class SystemRandomGenerator : IRandomGenerator
    {
        private Random random;

        /// <summary>
        /// Initializes a new instance of the SystemRandomGenerator class
        /// </summary>
        public SystemRandomGenerator()
        {
            // Courtesy of https://stackoverflow.com/questions/1785744/how-do-i-seed-a-random-class-to-avoid-getting-duplicate-random-values
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        /// <summary>
        /// Generates a random float within the given range
        /// </summary>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <returns>A random float between the minimum and maximum values.</returns>
        public float Random(float min, float max)
        {
            // Courtesy of https://stackoverflow.com/questions/3365337/best-way-to-generate-a-random-float-in-c-sharp

            // Perform arithmetic in double type to avoid overflowing
            double range = (double) max - (double) min;
            double sample = random.NextDouble();
            double scaled = (sample * range) + min;
            float result = (float) scaled;

            return result;
        }
    }
}