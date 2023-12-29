using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

namespace HereticalSolutions.Time.Visitors
{
    /// <summary>
    /// Represents a visitor for loading and saving <see cref="IPersistentTimer"/> objects.
    /// </summary>
    public class PersistentTimerVisitor
        : ILoadVisitorGeneric<IPersistentTimer, PersistentTimerDTO>,
          ILoadVisitor,
          ISaveVisitorGeneric<IPersistentTimer, PersistentTimerDTO>,
          ISaveVisitor
    {
        #region ILoadVisitorGeneric

        /// <summary>
        /// Loads data from a <see cref="PersistentTimerDTO"/> object into an <see cref="IPersistentTimer"/> object.
        /// </summary>
        /// <param name="DTO">The <see cref="PersistentTimerDTO"/> object to load data from.</param>
        /// <param name="value">The <see cref="IPersistentTimer"/> object to populate with the loaded data.</param>
        /// <returns><see langword="true"/> if loading was successful; otherwise, <see langword="false"/>.</returns>
        public bool Load(
            PersistentTimerDTO DTO,
            out IPersistentTimer value)
        {
            value = TimeFactory.BuildPersistentTimer(
                DTO.ID,
                DTO.DefaultDurationSpan);

            ((ITimerWithState)value).SetState(DTO.State);

            ((IPersistentTimerContext)value).StartTime = DTO.StartTime;

            ((IPersistentTimerContext)value).EstimatedFinishTime = DTO.EstimatedFinishTime;

            ((IPersistentTimerContext)value).SavedProgress = DTO.SavedProgress;

            ((IPersistentTimerContext)value).CurrentDurationSpan = DTO.CurrentDurationSpan;

            value.Accumulate = DTO.Accumulate;

            value.Repeat = DTO.Repeat;

            return true;
        }

        /// <summary>
        /// Loads data from a <see cref="PersistentTimerDTO"/> object into an <see cref="IPersistentTimer"/> object and populates the provided object.
        /// </summary>
        /// <param name="DTO">The <see cref="PersistentTimerDTO"/> object to load data from.</param>
        /// <param name="valueToPopulate">The <see cref="IPersistentTimer"/> object to populate with the loaded data.</param>
        /// <returns><see langword="true"/> if loading was successful; otherwise, <see langword="false"/>.</returns>
        public bool Load(
            PersistentTimerDTO DTO,
            IPersistentTimer valueToPopulate)
        {
            ((ITimerWithState)valueToPopulate).SetState(DTO.State);

            ((IPersistentTimerContext)valueToPopulate).StartTime = DTO.StartTime;

            ((IPersistentTimerContext)valueToPopulate).EstimatedFinishTime = DTO.EstimatedFinishTime;

            ((IPersistentTimerContext)valueToPopulate).SavedProgress = DTO.SavedProgress;

            ((IPersistentTimerContext)valueToPopulate).CurrentDurationSpan = DTO.CurrentDurationSpan;

            valueToPopulate.Accumulate = DTO.Accumulate;

            valueToPopulate.Repeat = DTO.Repeat;

            return true;
        }

        #endregion

        #region ILoadVisitor

        /// <summary>
        /// Loads data into a specified type of value from a specified DTO object.
        /// </summary>
        /// <typeparam name="TValue">The type of value to load.</typeparam>
        /// <param name="DTO">The DTO object to load data from.</param>
        /// <param name="value">The value to populate with the loaded data.</param>
        /// <returns><see langword="true"/> if loading was successful; otherwise, <see langword="false"/>.</returns>
        public bool Load<TValue>(object DTO, out TValue value)
        {
            if (!(DTO.GetType().Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");

            bool result = Load((PersistentTimerDTO)DTO, out IPersistentTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        /// <summary>
        /// Loads data into a specified type of value from a specified DTO object and populates the provided value.
        /// </summary>
        /// <typeparam name="TValue">The type of value to load.</typeparam>
        /// <typeparam name="TDTO">The type of DTO object to load data from.</typeparam>
        /// <param name="DTO">The DTO object to load data from.</param>
        /// <param name="value">The value to populate with the loaded data.</param>
        /// <returns><see langword="true"/> if loading was successful; otherwise, <see langword="false"/>.</returns>
        public bool Load<TValue, TDTO>(TDTO DTO, out TValue value)
        {
            if (!(typeof(TDTO).Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            // DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;

            bool result = Load((PersistentTimerDTO)dtoObject, out IPersistentTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        /// <summary>
        /// Loads data from a specified DTO object into a specified value and populates the provided value.
        /// </summary>
        /// <typeparam name="TValue">The type of value to load.</typeparam>
        /// <param name="DTO">The DTO object to load data from.</param>
        /// <param name="valueToPopulate">The value to populate with the loaded data.</param>
        /// <returns><see langword="true"/> if loading was successful; otherwise, <see langword="false"/>.</returns>
        public bool Load<TValue>(object DTO, TValue valueToPopulate)
        {
            if (!(DTO.GetType().Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");

            return Load((PersistentTimerDTO)DTO, (IPersistentTimer)valueToPopulate);
        }

        /// <summary>
        /// Loads data from a specified DTO object into a specified value and populates the provided value.
        /// </summary>
        /// <typeparam name="TValue">The type of value to load.</typeparam>
        /// <typeparam name="TDTO">The type of DTO object to load data from.</typeparam>
        /// <param name="DTO">The DTO object to load data from.</param>
        /// <param name="valueToPopulate">The value to populate with the loaded data.</param>
        /// <returns><see langword="true"/> if loading was successful; otherwise, <see langword="false"/>.</returns>
        public bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)
        {
            if (!(typeof(TDTO).Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            // DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;

            return Load((PersistentTimerDTO)dtoObject, (IPersistentTimer)valueToPopulate);
        }

        #endregion

        #region ISaveVisitorGeneric

        /// <summary>
        /// Saves data from an <see cref="IPersistentTimer"/> object into a <see cref="PersistentTimerDTO"/> object.
        /// </summary>
        /// <param name="value">The <see cref="IPersistentTimer"/> object to save data from.</param>
        /// <param name="DTO">The <see cref="PersistentTimerDTO"/> object to populate with the saved data.</param>
        /// <returns><see langword="true"/> if saving was successful; otherwise, <see langword="false"/>.</returns>
        public bool Save(
            IPersistentTimer value,
            out PersistentTimerDTO DTO)
        {
            DTO = new PersistentTimerDTO
            {
                ID = value.ID,
                State = value.State,
                StartTime = ((IPersistentTimerContext)value).StartTime,
                EstimatedFinishTime = ((IPersistentTimerContext)value).EstimatedFinishTime,
                SavedProgress = ((IPersistentTimerContext)value).SavedProgress,
                Accumulate = value.Accumulate,
                Repeat = value.Repeat,
                CurrentDurationSpan = value.CurrentDurationSpan,
                DefaultDurationSpan = value.DefaultDurationSpan
            };

            return true;
        }

        #endregion

        #region ISaveVisitor

        /// <summary>
        /// Saves data from a specified value into an object of type object.
        /// </summary>
        /// <typeparam name="TValue">The type of value to save.</typeparam>
        /// <param name="value">The value to save data from.</param>
        /// <param name="DTO">The object to populate with the saved data.</param>
        /// <returns><see langword="true"/> if saving was successful; otherwise, <see langword="false"/>.</returns>
        public bool Save<TValue>(TValue value, out object DTO)
        {
            if (!(typeof(IPersistentTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IPersistentTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");

            bool result = Save((IPersistentTimer)value, out PersistentTimerDTO returnDTO);

            DTO = result
                ? returnDTO
                : default(object);

            return result;
        }

        /// <summary>
        /// Saves data from a specified value into a specified DTO object.
        /// </summary>
        /// <typeparam name="TValue">The type of value to save.</typeparam>
        /// <typeparam name="TDTO">The type of DTO object to populate with the saved data.</typeparam>
        /// <param name="value">The value to save data from.</param>
        /// <param name="DTO">The DTO object to populate with the saved data.</param>
        /// <returns><see langword="true"/> if saving was successful; otherwise, <see langword="false"/>.</returns>
        public bool Save<TValue, TDTO>(TValue value, out TDTO DTO)
        {
            if (!(typeof(IPersistentTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IPersistentTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");

            bool result = Save((IPersistentTimer)value, out PersistentTimerDTO returnDTO);

            if (result)
            {
                // DIRTY HACKS DO NOT REPEAT
                var dtoObject = (object)returnDTO;

                DTO = (TDTO)dtoObject;
            }
            else
            {
                DTO = default(TDTO);
            }

            return result;
        }

        #endregion
    }
}