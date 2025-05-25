namespace DRG.Data
{
    public sealed class DataRecordFloat : ITypedDataRecord<float>
    {
        private readonly string keyCache;
        private readonly IDataProvider dataProvider;
        private float value;
        private bool hasValueCache;

        public DataRecordFloat(string key, float defaultValue, IDataProvider dataProvider)
        {
            keyCache = key;
            this.dataProvider = dataProvider;
            value = dataProvider.GetFloat(key, defaultValue);
            hasValueCache = dataProvider.ContainsKey(key);
        }

        public string key => keyCache;

        public bool hasValue => hasValueCache;

        public bool isDirty { get; private set; }

        /// <summary>
        /// Gets whether this record has been fully processed during save operations.
        /// Primarily used for object serialization tracking.
        /// Always returns true for float records.
        /// </summary>
        public bool processed => true;

        public void SetValue(float value)
        {
            if (this.value != value)
            {
                isDirty = true;
            }
            this.value = value;
            hasValueCache = true;
        }

        public float GetValue()
        {
            return value;
        }

        public void Apply()
        {
            if (isDirty)
            {
                dataProvider.SetFloat(key, value);
                isDirty = false;
            }
        }

        public void Erase()
        {
            dataProvider.DeleteKey(key);
            hasValueCache = false;
        }
    }
}