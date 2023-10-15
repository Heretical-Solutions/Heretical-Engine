using System;
using System.Collections.Generic;

namespace HereticalSolutions.Repositories.Factories
{
    /// <summary>
    /// Represents a factory class for creating repositories.
    /// </summary>
    public static class RepositoriesFactory
    {
        #region Dictionary object repository

        /// <summary>
        /// Builds a new instance of <see cref="DictionaryObjectRepository"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="DictionaryObjectRepository"/>.</returns>
        public static DictionaryObjectRepository BuildDictionaryObjectRepository()
        {
            return new DictionaryObjectRepository(
                BuildDictionaryRepository<Type, object>());
        }
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryObjectRepository"/> with the specified <paramref name="database"/>.
        /// </summary>
        /// <param name="database">The repository database.</param>
        /// <returns>A new instance of <see cref="DictionaryObjectRepository"/>.</returns>
        public static DictionaryObjectRepository BuildDictionaryObjectRepository(
            IRepository<Type, object> database)
        {
            return new DictionaryObjectRepository(
                database);
        }
        
        /// <summary>
        /// Clones an existing <see cref="IRepository{Type, object}"/> and creates a new instance of <see cref="DictionaryObjectRepository"/>.
        /// </summary>
        /// <param name="contents">The repository contents to clone.</param>
        /// <returns>A new instance of <see cref="DictionaryObjectRepository"/>.</returns>
        public static DictionaryObjectRepository CloneDictionaryObjectRepository(
            IRepository<Type, object> contents)
        {
            return new DictionaryObjectRepository(
                ((IClonableRepository<Type, object>)contents).Clone());
        }

        #endregion
        
        #region Dictionary repository
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>()
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>());
        }
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryRepository{TKey, TValue}"/> with the specified <paramref name="database"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <param name="database">The repository database.</param>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>(
            Dictionary<TKey, TValue> database)
        {
            return new DictionaryRepository<TKey, TValue>(
                database);
        }
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryRepository{TKey, TValue}"/> with the specified <paramref name="comparer"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>(
            IEqualityComparer<TKey> comparer)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(comparer));
        }
        
        /// <summary>
        /// Clones an existing <see cref="Dictionary{TKey, TValue}"/> and creates a new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <param name="contents">The repository contents to clone.</param>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> CloneDictionaryRepository<TKey, TValue>(
            Dictionary<TKey, TValue> contents)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(contents));
        }
        
        #endregion
    }
}