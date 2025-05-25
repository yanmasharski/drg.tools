namespace DRG.Data
{
    public interface IDataProvider
    {
        int GetInt(string key, int defaultValue);
        float GetFloat(string key, float defaultValue);
        string GetString(string key, string defaultValue);
        bool GetBool(string key, bool defaultValue);

        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetString(string key, string value);
        void SetBool(string key, bool value);

        bool ContainsKey(string key);
        void DeleteKey(string key);
        void DeleteAll();
        void Save();
    }
}