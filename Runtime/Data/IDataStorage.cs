namespace DRG.Data
{
    using DRG.Serialization;

    /// <summary>
    /// Interface for managing data storage and retrieval.
    /// </summary>
    public interface IDataStorage
    {
        /// <summary>
        /// Checks if a key exists in any of the data records.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets a boolean value for the specified key. Returns the default value if the key doesn't exist.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="defaultVal">The default value to return if the key doesn't exist.</param>
        /// <returns>The stored boolean value record.</returns>
        ITypedDataRecord<bool> GetBool(string key, bool defaultVal = false);

        /// <summary>
        /// Gets an integer value for the specified key. Returns the default value if the key doesn't exist.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="defaultVal">The default value to return if the key doesn't exist.</param>
        /// <returns>The stored integer value record.</returns>
        ITypedDataRecord<int> GetInt(string key, int defaultVal = 0);

        /// <summary>
        /// Gets a float value for the specified key. Returns the default value if the key doesn't exist.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="defaultVal">The default value to return if the key doesn't exist.</param>
        /// <returns>The stored float value record.</returns>
        ITypedDataRecord<float> GetFloat(string key, float defaultVal = 0f);

        /// <summary>
        /// Gets a string value for the specified key. Returns the default value if the key doesn't exist.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="defaultVal">The default value to return if the key doesn't exist.</param>
        /// <returns>The stored string value record.</returns>
        ITypedDataRecord<string> GetString(string key, string defaultVal = "");

        /// <summary>
        /// Gets an object value for the specified key. Returns the default value if the key doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="defaultVal">The default value to return if the key doesn't exist.</param>
        /// <returns>The stored object record.</returns>
        ITypedDataRecord<T> GetObject<T>(string key, T defaultVal, IDataSerializer serializer);

        /// <summary>
        /// Deletes a key and its associated value from all records.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        void Erase(string key);

        /// <summary>
        /// Deletes all keys and their associated values from all records.
        /// </summary>
        void EraseAll();

        /// <summary>
        /// Saves all dirty records. Can be delayed by specifying a frame cooldown.
        /// </summary>
        /// <param name="framesCooldown">Number of frames to wait before saving. If greater than 0,
        /// the save operation will be performed after the specified number of frames.</param>
        void Save(int framesCooldown = 60);
    }
}
