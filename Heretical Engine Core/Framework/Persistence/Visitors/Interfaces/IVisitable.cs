using System;

namespace HereticalSolutions.Persistence
{
    /// <summary>
    /// Represents an interface that allows an object to be visited by a visitor
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Gets the type of the data transfer object (DTO) associated with the visitable object
        /// </summary>
        Type DTOType { get; }

        /// <summary>
        /// Accepts a save visitor and returns the corresponding DTO of type <typeparamref name="TDTO"/>
        /// </summary>
        /// <typeparam name="TDTO">The type of the data transfer object (DTO) to return.</typeparam>
        /// <param name="visitor">The save visitor to accept.</param>
        /// <param name="DTO">When this method returns, contains the corresponding DTO if the visitor is successful; otherwise, the default value for type <typeparamref name="TDTO"/>.</param>
        /// <returns><c>true</c> if the visitor is successful; otherwise, <c>false</c>.</returns>
        bool Accept<TDTO>(ISaveVisitor visitor, out TDTO DTO);
        
        /// <summary>
        /// Accepts a save visitor and returns the corresponding DTO of type <see cref="object"/>
        /// </summary>
        /// <param name="visitor">The save visitor to accept.</param>
        /// <param name="DTO">When this method returns, contains the corresponding DTO if the visitor is successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the visitor is successful; otherwise, <c>false</c>.</returns>
        bool Accept(ISaveVisitor visitor, out object DTO);
        
        /// <summary>
        /// Accepts a load visitor and performs the visitation using the provided DTO of type <typeparamref name="TDTO"/>
        /// </summary>
        /// <typeparam name="TDTO">The type of the data transfer object (DTO) to use.</typeparam>
        /// <param name="visitor">The load visitor to accept.</param>
        /// <param name="DTO">The DTO to use for visitation.</param>
        /// <returns><c>true</c> if the visitor is successful; otherwise, <c>false</c>.</returns>
        bool Accept<TDTO>(ILoadVisitor visitor, TDTO DTO);
        
        /// <summary>
        /// Accepts a load visitor and performs the visitation using the provided DTO of type <see cref="object"/>
        /// </summary>
        /// <param name="visitor">The load visitor to accept.</param>
        /// <param name="DTO">The DTO to use for visitation.</param>
        /// <returns><c>true</c> if the visitor is successful; otherwise, <c>false</c>.</returns>
        bool Accept(ILoadVisitor visitor, object DTO);
    }
}