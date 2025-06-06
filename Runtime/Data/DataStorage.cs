namespace DRG.Data
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using DRG.Utils;
    using DRG.Serialization;
    using ILogger = Logs.ILogger;

    /// <summary>
    /// DataStorage is a high-performance wrapper around Unity's PlayerPrefs system.
    /// It provides an in-memory cache with dirty tracking to minimize disk operations
    /// and improve performance when dealing with persistent data.
    /// Additionally, this system enforces structured data access through the DataRecord
    /// interface, which enhances code organization, facilitates debugging, and enables
    /// robust data validation.
    /// </summary>
    public class DataStorage : IEnumerable<IDataRecord>, IDataStorage
    {
        private readonly Dictionary<string, ITypedDataRecord<int>> recordsInt = new Dictionary<string, ITypedDataRecord<int>>();
        private readonly Dictionary<string, ITypedDataRecord<bool>> recordsBool = new Dictionary<string, ITypedDataRecord<bool>>();
        private readonly Dictionary<string, ITypedDataRecord<float>> recordsFloat = new Dictionary<string, ITypedDataRecord<float>>();
        private readonly Dictionary<string, ITypedDataRecord<string>> recordsString = new Dictionary<string, ITypedDataRecord<string>>();
        private readonly Dictionary<string, DataRecordObject> recordsObject = new Dictionary<string, DataRecordObject>(); // TODO: Think how to migrate to interfaces
        private readonly object lockObject = new object();

        private readonly IDataProvider dataProvider;
        private readonly ILogger logger;
        private readonly IDebouncedExecutor debouncedExecutor;

        private DataStorage(IDataProvider dataProvider, ILogger logger, IDebouncedExecutor debouncedExecutor)
        {
            this.dataProvider = dataProvider;
            this.logger = logger;
            this.debouncedExecutor = debouncedExecutor;
        }

        public static DataStorage current { get; private set; }

        public static void Init(IDataProvider dataProvider, ILogger logger, IDebouncedExecutor debouncedExecutor)
        {
            current = new DataStorage(dataProvider, logger, debouncedExecutor);
        }

        public bool ContainsKey(string key)
        {
            return (recordsInt.TryGetValue(key, out var intRecord) && intRecord.hasValue) ||
                (recordsBool.TryGetValue(key, out var boolRecord) && boolRecord.hasValue) ||
                (recordsFloat.TryGetValue(key, out var floatRecord) && floatRecord.hasValue) ||
                (recordsString.TryGetValue(key, out var stringRecord) && stringRecord.hasValue) ||
                (recordsObject.TryGetValue(key, out var objectRecord) && objectRecord.hasValue) ||
                dataProvider.ContainsKey(key);
        }

        public ITypedDataRecord<bool> GetBool(string key, bool defaultValue = default)
        {
            lock (lockObject)
            {
                if (!recordsBool.TryGetValue(key, out var result))
                {
                    result = new DataRecordBool(key, defaultValue, dataProvider);
                    recordsBool[key] = result;
                }

                return result;
            }
        }

        public ITypedDataRecord<int> GetInt(string key, int defaultVal = 0)
        {
            lock (lockObject)
            {
                if (!recordsInt.TryGetValue(key, out var result))
                {
                    result = new DataRecordInt(key, defaultVal, dataProvider);
                    recordsInt[key] = result;
                }

                return result;
            }
        }

        public ITypedDataRecord<float> GetFloat(string key, float defaultVal = 0f)
        {
            lock (lockObject)
            {
                if (!recordsFloat.TryGetValue(key, out var result))
                {
                    result = new DataRecordFloat(key, defaultVal, dataProvider);
                    recordsFloat[key] = result;
                }

                return result;
            }
        }

        public ITypedDataRecord<string> GetString(string key, string defaultVal = "")
        {
            lock (lockObject)
            {
                if (!recordsString.TryGetValue(key, out var result))
                {
                    result = new DataRecordString(key, defaultVal, dataProvider, logger);
                    recordsString[key] = result;
                }

                return result;
            }
        }

        public ITypedDataRecord<T> GetObject<T>(string key, T defaultVal, IDataSerializer serializer)
        {
            lock (lockObject)
            {
                if (!recordsObject.TryGetValue(key, out var result))
                {
                    result = new DataRecordObject(key, typeof(T), defaultVal, serializer, dataProvider);
                    recordsObject[key] = result;
                }

                return result.GetTypedDataRecord<T>();
            }
        }

        public void Erase(string key)
        {
            lock (lockObject)
            {
                if (TryGetValue(key, out var record))
                {
                    record.Erase();
                }
            }

            dataProvider.DeleteKey(key);
        }

        public void EraseAll()
        {
            foreach (var record in this)
            {
                record.Erase();
            }

            recordsInt.Clear();
            recordsBool.Clear();
            recordsFloat.Clear();
            recordsString.Clear();
            recordsObject.Clear();
        }

        public void Save(int framesCooldown = 60)
        {
            debouncedExecutor.Execute(framesCooldown, ProcessAndSave());
        }

        public IEnumerator<IDataRecord> GetEnumerator()
        {
            foreach (var record in recordsInt)
            {
                yield return record.Value;
            }

            foreach (var record in recordsBool)
            {
                yield return record.Value;
            }

            foreach (var record in recordsFloat)
            {
                yield return record.Value;
            }

            foreach (var record in recordsString)
            {
                yield return record.Value;
            }

            foreach (var record in recordsObject)
            {
                yield return record.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool TryGetValue(string key, out IDataRecord record)
        {
            if (recordsInt.TryGetValue(key, out var resultInt))
            {
                record = resultInt;
                return true;
            }

            if (recordsBool.TryGetValue(key, out var resultBool))
            {
                record = resultBool;
                return true;
            }

            if (recordsFloat.TryGetValue(key, out var resultFloat))
            {
                record = resultFloat;
                return true;
            }

            if (recordsString.TryGetValue(key, out var resultString))
            {
                record = resultString;
                return true;
            }

            if (recordsObject.TryGetValue(key, out var resultObject))
            {
                record = resultObject;
                return true;
            }

            record = null;
            return false;
        }

        private IEnumerator ProcessAndSave()
        {
            // Force save all records
            var hasChanges = false;

            lock (lockObject)
            {
                foreach (var record in this)
                {
                    hasChanges |= record.isDirty;
                    record.Apply();
                }
            }

            if (!hasChanges)
            {
                yield break;
            }

            // Wait for all records of objects to be processed
            while (true)
            {
                bool processed;
                lock (lockObject)
                {
                    processed = true;
                    foreach (var record in recordsObject)
                    {
                        processed &= record.Value.processed;
                    }

                    if (processed || recordsObject.Count == 0)
                    {
                        break;
                    }
                }

                yield return null;
            }

            dataProvider.Save();
        }

    }
}