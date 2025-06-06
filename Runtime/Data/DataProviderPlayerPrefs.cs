namespace DRG.Data
{
    using Utils;
    using UnityEngine;

    /// <summary>
    /// DataProviderPlayerPrefs is a class that provides a way to store and retrieve data using PlayerPrefs.
    /// Works only on main thread.
    /// </summary>
    public class DataProviderPlayerPrefs : IDataProvider
    {
        private readonly bool threadSafe;

        public DataProviderPlayerPrefs(bool threadSafe = false)
        {
#if UNITY_IOS
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            this.threadSafe = threadSafe;
        }

        public int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        public void SetInt(string key, int value)
        {
            if (threadSafe)
            {
                MainThreadDispatcher.Enqueue(OnMainThread);
            }
            else
            {
                OnMainThread();
            }

            return;

            void OnMainThread()
            {
                PlayerPrefs.SetInt(key, value);
            }
        }

        public void SetFloat(string key, float value)
        {
            if (threadSafe)
            {
                MainThreadDispatcher.Enqueue(OnMainThread);
            }
            else
            {
                OnMainThread();
            }

            return;

            void OnMainThread()
            {
                PlayerPrefs.SetFloat(key, value);
            }
        }

        public void SetString(string key, string value)
        {
            if (threadSafe)
            {
                MainThreadDispatcher.Enqueue(OnMainThread);
            }
            else
            {
                OnMainThread();
            }

            return;

            void OnMainThread()
            {
                PlayerPrefs.SetString(key, value);
            }
        }

        public void SetBool(string key, bool value)
        {
            if (threadSafe)
            {
                MainThreadDispatcher.Enqueue(OnMainThread);
            }
            else
            {
                OnMainThread();
            }

            return;

            void OnMainThread()
            {
                PlayerPrefs.SetInt(key, value ? 1 : 0);
            }
        }

        public bool ContainsKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }
    }
}