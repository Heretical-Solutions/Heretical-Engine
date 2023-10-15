using System;
using System.Runtime.InteropServices;

using HereticalSolutions.Collections.Unmanaged;

namespace HereticalSolutions.Collections.Factories
{
    public static partial class CollectionsFactory
    {
        /// <summary>
        /// Builds an instance of UnmanagedArray.
        /// </summary>
        /// <param name="memoryPointer">A pointer to the memory location of the array.</param>
        /// <param name="elementSize">The size of each element in bytes.</param>
        /// <param name="elementCapacity">The initial capacity of the array.</param>
        /// <returns>An instance of UnmanagedArray.</returns>
        public unsafe static UnmanagedArray BuildUnmanagedArray(
            byte* memoryPointer,
            int elementSize,
            int elementCapacity = 0)
        {
            return new UnmanagedArray(
                memoryPointer,
                elementSize * elementCapacity,
                elementSize,
                elementCapacity);
        }

        /// <summary>
        /// Builds an instance of UnmanagedArray using the generic type parameter.
        /// </summary>
        /// <typeparam name="T">The type of the elements stored in the array.</typeparam>
        /// <param name="memoryPointer">A pointer to the memory location of the array.</param>
        /// <param name="elementCapacity">The initial capacity of the array.</param>
        /// <returns>An instance of UnmanagedArray.</returns>
        public unsafe static UnmanagedArray BuildUnmanagedArrayGeneric<T>(
            byte* memoryPointer,
            int elementCapacity = 0)
        {
            int elementSize = Marshal.SizeOf(typeof(T));

            return new UnmanagedArray(
                memoryPointer,
                elementSize * elementCapacity,
                elementSize,
                elementCapacity);
        }

        /// <summary>
        /// Resizes an existing UnmanagedArray.
        /// </summary>
        /// <param name="array">The UnmanagedArray to resize.</param>
        /// <param name="newMemoryPointer">A pointer to the new memory location of the array.</param>
        /// <param name="newElementCapacity">The new capacity of the array.</param>
        public unsafe static void ResizeUnmanagedArray(
            ref UnmanagedArray array,
            byte* newMemoryPointer,
            int newElementCapacity)
        {
            Buffer.MemoryCopy(
                array.MemoryPointer,
                newMemoryPointer,
                newElementCapacity * array.ElementSize,
                array.ElementCapacity * array.ElementSize);

            array.ElementCapacity = newElementCapacity;
        }
    }
}