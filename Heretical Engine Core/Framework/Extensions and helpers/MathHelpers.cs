namespace HereticalSolutions
{
    /// <summary>
    /// Provides mathematical helper functions.
    /// </summary>
    public static class MathHelpers
    {
        //TODO: add stuff from OpenTK

        /// <summary>
        /// Defines a small constant value used for floating-point comparisons.
        /// </summary>
        public const float EPSILON = 0.0001f;

        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }
    }
}