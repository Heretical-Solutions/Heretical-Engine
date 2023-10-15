using System;
using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type is enumerable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsTypeEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the specified type is a generic enumerable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is a generic enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsTypeGenericEnumerable(this Type type)
        {
            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Gets the underlying type of a generic enumerable.
        /// </summary>
        /// <param name="type">The type to get the underlying type of.</param>
        /// <returns>The underlying type of the generic enumerable.</returns>
        public static Type GetGenericEnumerableUnderlyingType(this Type type)
        {
            return type.GetGenericArguments()[0];
        }

        /// <summary>
        /// Determines whether the specified type is a generic array.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is a generic array; otherwise, <c>false</c>.</returns>
        public static bool IsTypeGenericArray(this Type type)
        {
            return type.IsArray;
        }

        /// <summary>
        /// Gets the underlying type of a generic array.
        /// </summary>
        /// <param name="type">The type to get the underlying type of.</param>
        /// <returns>The underlying type of the generic array.</returns>
        public static Type GetGenericArrayUnderlyingType(this Type type)
        {
            return type.GetElementType();
        }
    }
}