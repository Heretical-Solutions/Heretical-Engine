using System;
using System.Linq;

/// <summary>
/// Extension methods for the QuestStage class
/// </summary>
namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Provides extension methods for the QuestStage class
    /// </summary>
    public static class QuestsExtensions
    {
        /// <summary>
        /// Retrieves an integer value from the specified property ID in the quest stage
        /// </summary>
        /// <param name="stage">The quest stage.</param>
        /// <param name="id">The ID of the property.</param>
        /// <returns>The integer value of the property.</returns>
        /// <exception cref="Exception">Thrown when the quest stage does not have a property with the specified ID.</exception>
        public static int GetInt(this QuestStage stage, string id)
        {
            var property = stage.Properties.FirstOrDefault(item => item.Key == id);

            if (property.Equals(default(QuestPropertyDTO)))
                throw new Exception(string.Format("[QuestExtensions] QUEST STAGE HAS NO PROPERTY BY THE KEY {0}", id));

            return Convert.ToInt32(property.Value);
        }

        /// <summary>
        /// Retrieves a float value from the specified property ID in the quest stage
        /// </summary>
        /// <param name="stage">The quest stage.</param>
        /// <param name="id">The ID of the property.</param>
        /// <returns>The float value of the property.</returns>
        /// <exception cref="Exception">Thrown when the quest stage does not have a property with the specified ID.</exception>
        public static float GetFloat(this QuestStage stage, string id)
        {
            var property = stage.Properties.FirstOrDefault(item => item.Key == id);

            if (property.Equals(default(QuestPropertyDTO)))
                throw new Exception(string.Format("[QuestExtensions] QUEST STAGE HAS NO PROPERTY BY THE KEY {0}", id));

            return Convert.ToSingle(property.Value);
        }

        /// <summary>
        /// Retrieves a string value from the specified property ID in the quest stage
        /// </summary>
        /// <param name="stage">The quest stage.</param>
        /// <param name="id">The ID of the property.</param>
        /// <returns>The string value of the property.</returns>
        /// <exception cref="Exception">Thrown when the quest stage does not have a property with the specified ID.</exception>
        public static string GetString(this QuestStage stage, string id)
        {
            var property = stage.Properties.FirstOrDefault(item => item.Key == id);

            if (property.Equals(default(QuestPropertyDTO)))
                throw new Exception(string.Format("[QuestExtensions] QUEST STAGE HAS NO PROPERTY BY THE KEY {0}", id));

            return property.Value;
        }

        /// <summary>
        /// Retrieves a QuestPropertyDTO object by the specified key from the quest property array
        /// </summary>
        /// <param name="properties">The quest property array.</param>
        /// <param name="key">The key of the property.</param>
        /// <returns>The QuestPropertyDTO object with the specified key.</returns>
        public static QuestPropertyDTO ByKey(this QuestPropertyDTO[] properties, string key)
        {
            return properties.FirstOrDefault(item => item.Key == key);
        }
    }
}