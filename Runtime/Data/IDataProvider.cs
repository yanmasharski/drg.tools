namespace DRG.Data
{
    /// <summary>
    /// Interface for providing persistent data storage functionality.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Gets an integer value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if key is not found.</param>
        /// <returns>The stored integer value, or defaultValue if key not found.</returns>
        int GetInt(string key, int defaultValue);

        /// <summary>
        /// Gets a float value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if key is not found.</param>
        /// <returns>The stored float value, or defaultValue if key not found.</returns>
        float GetFloat(string key, float defaultValue);

        /// <summary>
        /// Gets a string value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if key is not found.</param>
        /// <returns>The stored string value, or defaultValue if key not found.</returns>
        string GetString(string key, string defaultValue);

        /// <summary>
        /// Gets a boolean value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if key is not found.</param>
        /// <returns>The stored boolean value, or defaultValue if key not found.</returns>
        bool GetBool(string key, bool defaultValue);

        /// <summary>
        /// Sets an integer value for the specified key.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The integer value to store.</param>
        void SetInt(string key, int value);

        /// <summary>
        /// Sets a float value for the specified key.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The float value to store.</param>
        void SetFloat(string key, float value);

        /// <summary>
        /// Sets a string value for the specified key.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The string value to store.</param>
        void SetString(string key, string value);

        /// <summary>
        /// Sets a boolean value for the specified key.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The boolean value to store.</param>
        void SetBool(string key, bool value);

        /// <summary>
        /// Checks if a value exists for the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Deletes the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        void DeleteKey(string key);

        /// <summary>
        /// Deletes all stored key-value pairs.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Saves all pending changes to persistent storage.
        /// </summary>
        void Save();
    }
}