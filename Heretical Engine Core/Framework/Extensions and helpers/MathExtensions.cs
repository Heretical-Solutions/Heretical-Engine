using System;

namespace HereticalSolutions
{
    /// <summary>
    /// Provides extension methods for mathematical operations.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <typeparam name="T">The type of the value to clamp.</typeparam>
        /// <param name="val">The value to clamp.</param>
        /// <param name="min">The minimum value to clamp to.</param>
        /// <param name="max">The maximum value to clamp to.</param>
        /// <returns>The clamped value.</returns>
        /// <remarks>
        /// If the value is less than the minimum, it will be set to the minimum.
        /// If the value is greater than the maximum, it will be set to the maximum.
        /// </remarks>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
                return min;
            
            if(val.CompareTo(max) > 0)
                return max;
            
            return val;
        }
    }
}