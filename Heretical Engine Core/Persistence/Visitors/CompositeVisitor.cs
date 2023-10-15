using System;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Visitors
{
    /// <summary>
    /// Represents a composite visitor that implements both ISaveVisitor and ILoadVisitor interfaces.
    /// </summary>
    public class CompositeVisitor : ISaveVisitor, ILoadVisitor
    {
        private readonly IReadOnlyObjectRepository loadVisitorsRepository;
        private readonly IReadOnlyObjectRepository saveVisitorsRepository;

        /// <summary>
        /// Initializes a new instance of the CompositeVisitor class.
        /// </summary>
        /// <param name="loadVisitorsRepository">The repository containing the load visitors.</param>
        /// <param name="saveVisitorsRepository">The repository containing the save visitors.</param>
        public CompositeVisitor(
            IReadOnlyObjectRepository loadVisitorsRepository,
            IReadOnlyObjectRepository saveVisitorsRepository)
        {
            this.loadVisitorsRepository = loadVisitorsRepository;
            this.saveVisitorsRepository = saveVisitorsRepository;
        }

        #region ISaveVisitor

        /// <summary>
        /// Saves the specified value using the appropriate save visitor.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be saved.</typeparam>
        /// <param name="value">The value to be saved.</param>
        /// <param name="DTO">The data transfer object that represents the saved value.</param>
        /// <returns>true if the value was saved successfully; otherwise, false.</returns>
        public bool Save<TValue>(TValue value, out object DTO)
        {
            DTO = default(object);

            if (!saveVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\"");

            var concreteVisitor = (ISaveVisitor)concreteVisitorObject;

            return concreteVisitor.Save(value, out DTO);
        }

        /// <summary>
        /// Saves the specified value using the appropriate save visitor.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be saved.</typeparam>
        /// <typeparam name="TDTO">The type of the data transfer object that represents the saved value.</typeparam>
        /// <param name="value">The value to be saved.</param>
        /// <param name="DTO">The data transfer object that represents the saved value.</param>
        /// <returns>true if the value was saved successfully; otherwise, false.</returns>
        public bool Save<TValue, TDTO>(TValue value, out TDTO DTO)
        {
            DTO = default(TDTO);

            if (!saveVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\" AND DTO TYPE \"{typeof(TDTO).ToString()}\"");

            var concreteVisitor = (ISaveVisitorGeneric<TValue, TDTO>)concreteVisitorObject;

            return concreteVisitor.Save(value, out DTO);
        }

        #endregion

        #region ILoadVisitor

        /// <summary>
        /// Loads a value using the appropriate load visitor.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be loaded.</typeparam>
        /// <param name="DTO">The data transfer object that represents the value to be loaded.</param>
        /// <param name="value">The loaded value.</param>
        /// <returns>true if the value was loaded successfully; otherwise, false.</returns>
        public bool Load<TValue>(object DTO, out TValue value)
        {
            value = default(TValue);

            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\"");

            var concreteVisitor = (ILoadVisitor)concreteVisitorObject;

            return concreteVisitor.Load(DTO, out value);
        }

        /// <summary>
        /// Loads a value using the appropriate load visitor.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be loaded.</typeparam>
        /// <typeparam name="TDTO">The type of the data transfer object that represents the value to be loaded.</typeparam>
        /// <param name="DTO">The data transfer object that represents the value to be loaded.</param>
        /// <param name="value">The loaded value.</param>
        /// <returns>true if the value was loaded successfully; otherwise, false.</returns>
        public bool Load<TValue, TDTO>(TDTO DTO, out TValue value)
        {
            value = default(TValue);

            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\" AND DTO TYPE \"{typeof(TDTO).ToString()}\"");

            var concreteVisitor = (ILoadVisitorGeneric<TValue, TDTO>)concreteVisitorObject;

            return concreteVisitor.Load(DTO, out value);
        }

        /// <summary>
        /// Loads a value using the appropriate load visitor and populates an existing value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be loaded.</typeparam>
        /// <param name="DTO">The data transfer object that represents the value to be loaded.</param>
        /// <param name="valueToPopulate">The value to be populated with the loaded data.</param>
        /// <returns>true if the value was loaded and populated successfully; otherwise, false.</returns>
        public bool Load<TValue>(object DTO, TValue valueToPopulate)
        {
            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\"");

            var concreteVisitor = (ILoadVisitor)concreteVisitorObject;

            return concreteVisitor.Load(DTO, valueToPopulate);
        }

        /// <summary>
        /// Loads a value using the appropriate load visitor and populates an existing value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be loaded.</typeparam>
        /// <typeparam name="TDTO">The type of the data transfer object that represents the value to be loaded.</typeparam>
        /// <param name="DTO">The data transfer object that represents the value to be loaded.</param>
        /// <param name="valueToPopulate">The value to be populated with the loaded data.</param>
        /// <returns>true if the value was loaded and populated successfully; otherwise, false.</returns>
        public bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)
        {
            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\" AND DTO TYPE \"{typeof(TDTO).ToString()}\"");

            var concreteVisitor = (ILoadVisitorGeneric<TValue, TDTO>)concreteVisitorObject;

            return concreteVisitor.Load(DTO, valueToPopulate);
        }

        #endregion
    }
}