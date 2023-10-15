using System;
using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

namespace HereticalSolutions.Time.Visitors
{
    /// <summary>
    /// Visitor for loading and saving runtime timers.
    /// </summary>
    public class RuntimeTimerVisitor : ILoadVisitorGeneric<IRuntimeTimer, RuntimeTimerDTO>, ILoadVisitor, ISaveVisitorGeneric<IRuntimeTimer, RuntimeTimerDTO>, ISaveVisitor
    {
        #region ILoadVisitorGeneric

        /// <summary>
        /// Loads a runtime timer from a Data Transfer Object (DTO).
        /// </summary>
        /// <param name="DTO">The DTO containing the runtime timer data.</param>
        /// <param name="value">The loaded runtime timer.</param>
        /// <returns>True if the loading is successful, false otherwise.</returns>
        public bool Load(RuntimeTimerDTO DTO, out IRuntimeTimer value)
        {
            value = TimeFactory.BuildRuntimeTimer(
                DTO.ID,
                DTO.DefaultDuration);

            ((ITimerWithState)value).SetState(DTO.State);

            ((IRuntimeTimerContext)value).CurrentTimeElapsed = DTO.CurrentTimeElapsed;

            ((IRuntimeTimerContext)value).CurrentDuration = DTO.CurrentDuration;

            value.Accumulate = DTO.Accumulate;

            value.Repeat = DTO.Repeat;

            return true;
        }

        /// <summary>
        /// Loads a runtime timer from a Data Transfer Object (DTO).
        /// </summary>
        /// <param name="DTO">The DTO containing the runtime timer data.</param>
        /// <param name="valueToPopulate">The runtime timer to be populated with the loaded data.</param>
        /// <returns>True if the loading is successful, false otherwise.</returns>
        public bool Load(RuntimeTimerDTO DTO, IRuntimeTimer valueToPopulate)
        {
            ((ITimerWithState)valueToPopulate).SetState(DTO.State);

            ((IRuntimeTimerContext)valueToPopulate).CurrentTimeElapsed = DTO.CurrentTimeElapsed;

            ((IRuntimeTimerContext)valueToPopulate).CurrentDuration = DTO.CurrentDuration;

            valueToPopulate.Accumulate = DTO.Accumulate;

            valueToPopulate.Repeat = DTO.Repeat;

            return true;
        }

        #endregion

        #region ILoadVisitor

        /// <summary>
        /// Loads a runtime timer from a Data Transfer Object (DTO).
        /// </summary>
        /// <typeparam name="TValue">The type of the loaded value.</typeparam>
        /// <param name="DTO">The DTO containing the runtime timer data.</param>
        /// <param name="value">The loaded runtime timer.</param>
        /// <returns>True if the loading is successful, false otherwise.</returns>
        public bool Load<TValue>(object DTO, out TValue value)
        {
            if (!(DTO.GetType().Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");

            bool result = Load((RuntimeTimerDTO)DTO, out IRuntimeTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        /// <summary>
        /// Loads a runtime timer from a Data Transfer Object (DTO).
        /// </summary>
        /// <typeparam name="TValue">The type of the loaded value.</typeparam>
        /// <typeparam name="TDTO">The type of the DTO.</typeparam>
        /// <param name="DTO">The DTO containing the runtime timer data.</param>
        /// <param name="value">The loaded runtime timer.</param>
        /// <returns>True if the loading is successful, false otherwise.</returns>
        public bool Load<TValue, TDTO>(TDTO DTO, out TValue value)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            // DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;

            bool result = Load((RuntimeTimerDTO)dtoObject, out IRuntimeTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        /// <summary>
        /// Loads a runtime timer from a Data Transfer Object (DTO).
        /// </summary>
        /// <typeparam name="TValue">The type of the value to populate.</typeparam>
        /// <param name="DTO">The DTO containing the runtime timer data.</param>
        /// <param name="valueToPopulate">The runtime timer to be populated with the loaded data.</param>
        /// <returns>True if the loading is successful, false otherwise.</returns>
        public bool Load<TValue>(object DTO, TValue valueToPopulate)
        {
            if (!(DTO.GetType().Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");

            return Load((RuntimeTimerDTO)DTO, (IRuntimeTimer)valueToPopulate);
        }

        /// <summary>
        /// Loads a runtime timer from a Data Transfer Object (DTO).
        /// </summary>
        /// <typeparam name="TValue">The type of the value to populate.</typeparam>
        /// <typeparam name="TDTO">The type of the DTO.</typeparam>
        /// <param name="DTO">The DTO containing the runtime timer data.</param>
        /// <param name="valueToPopulate">The runtime timer to be populated with the loaded data.</param>
        /// <returns>True if the loading is successful, false otherwise.</returns>
        public bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            // DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;

            return Load((RuntimeTimerDTO)dtoObject, (IRuntimeTimer)valueToPopulate);
        }

        #endregion

        #region ISaveVisitorGeneric

        /// <summary>
        /// Saves a runtime timer to a Data Transfer Object (DTO).
        /// </summary>
        /// <param name="value">The runtime timer to be saved.</param>
        /// <param name="DTO">The saved DTO.</param>
        /// <returns>True if the saving is successful, false otherwise.</returns>
        public bool Save(IRuntimeTimer value, out RuntimeTimerDTO DTO)
        {
            DTO = new RuntimeTimerDTO
            {
                ID = value.ID,
                State = value.State,
                CurrentTimeElapsed = ((IRuntimeTimerContext)value).CurrentTimeElapsed,
                Accumulate = value.Accumulate,
                Repeat = value.Repeat,
                CurrentDuration = value.CurrentDuration,
                DefaultDuration = value.DefaultDuration
            };

            return true;
        }

        #endregion

        #region ISaveVisitor

        /// <summary>
        /// Saves a runtime timer to a Data Transfer Object (DTO).
        /// </summary>
        /// <typeparam name="TValue">The type of the value to save.</typeparam>
        /// <param name="value">The runtime timer to be saved.</param>
        /// <param name="DTO">The saved DTO.</param>
        /// <returns>True if the saving is successful, false otherwise.</returns>
        public bool Save<TValue>(TValue value, out object DTO)
        {
            if (!(typeof(IRuntimeTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IRuntimeTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");

            bool result = Save((IRuntimeTimer)value, out RuntimeTimerDTO returnDTO);

            DTO = result
                ? returnDTO
                : default(object);

            return result;
        }

        /// <summary>
        /// Saves a runtime timer to a Data Transfer Object (DTO).
        /// </summary>
        /// <typeparam name="TValue">The type of the value to save.</typeparam>
        /// <typeparam name="TDTO">The type of the DTO.</typeparam>
        /// <param name="value">The runtime timer to be saved.</param>
        /// <param name="DTO">The saved DTO.</param>
        /// <returns>True if the saving is successful, false otherwise.</returns>
        public bool Save<TValue, TDTO>(TValue value, out TDTO DTO)
        {
            if (!(typeof(IRuntimeTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IRuntimeTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");

            bool result = Save((IRuntimeTimer)value, out RuntimeTimerDTO returnDTO);

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