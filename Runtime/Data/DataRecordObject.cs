namespace DRG.Data
{
    using System;
    using DRG.Serialization;
    using DRG.Utils;

    public sealed class DataRecordObject : ITypedDataRecord<object>
    {
        private readonly string keyCache;
        private readonly IDataSerializer serializer;
        private readonly Type type;
        private readonly IDataProvider dataProvider;
        private object value;
        private bool hasValueCache;
        public DataRecordObject(string key, Type type, object defaultValue, IDataSerializer serializer, IDataProvider dataProvider)
        {
            keyCache = key;
            this.type = type;
            this.serializer = serializer;
            this.dataProvider = dataProvider;
            hasValueCache = dataProvider.ContainsKey(key);
            value = hasValueCache ? serializer.Deserialize(type, dataProvider.GetString(key, string.Empty)) : defaultValue;
        }

        public string key => keyCache;
        public bool hasValue => hasValueCache;

        public bool isDirty { get; private set; }

        public bool processed { get; private set; }

        public void SetValue(object value)
        {
            if (!this.value.Equals(value))
            {
                isDirty = true;
            }

            this.value = value;
            hasValueCache = true;
            processed = false;
        }

        public object GetValue()
        {
            return value;
        }

        public ITypedDataRecord<T> GetTypedDataRecord<T>()
        {
            return new DataRecordObjectProxy<T>(this);
        }

        public void Apply()
        {
            if (!isDirty)
            {
                return;
            }

            System.Threading.Tasks.Task.Run(SerializationTask);

            void SerializationTask()
            {
                string serializedData = serializer.Serialize(value);
                MainThreadDispatcher.Enqueue(OnMainThread);
                void OnMainThread()
                {
                    dataProvider.SetString(key, serializedData);
                    processed = true;
                }
            }
        }

        public void Erase()
        {
            dataProvider.DeleteKey(key);
            hasValueCache = false;
        }
        private class DataRecordObjectProxy<T> : ITypedDataRecord<T>
        {
            private readonly DataRecordObject dataRecordObject;

            public DataRecordObjectProxy(DataRecordObject dataRecordObject)
            {
                this.dataRecordObject = dataRecordObject;
            }

            public string key => dataRecordObject.key;
            public bool hasValue => dataRecordObject.hasValue;
            public bool isDirty => dataRecordObject.isDirty;
            public bool processed => dataRecordObject.processed;

            public void SetValue(T value)
            {
                dataRecordObject.SetValue(value);
            }

            public T GetValue()
            {
                return (T)dataRecordObject.GetValue();
            }

            public void Apply()
            {
                dataRecordObject.Apply();
            }

            public void Erase()
            {
                dataRecordObject.Erase();
            }

        }
    }
}

