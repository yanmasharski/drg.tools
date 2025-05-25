namespace DRG.Data
{
    public sealed class DataRecordInt : ITypedDataRecord<int>
    {
        private readonly string keyCache;
        private readonly IDataProvider dataProvider;
        private int value;
        private bool hasValueCache;

        public DataRecordInt(string key, int defaultValue, IDataProvider dataProvider)
        {
            keyCache = key;
            this.dataProvider = dataProvider;
            value = dataProvider.GetInt(key, defaultValue);
            hasValueCache = dataProvider.ContainsKey(key);
        }

        public string key => keyCache;

        public bool hasValue => hasValueCache;

        public bool isDirty { get; private set; }

        /// <summary>
        /// Gets whether this record has been fully processed during save operations.
        /// Primarily used for object serialization tracking.
        /// Always returns true for int records.
        /// </summary>
        public bool processed => true;

        public void SetValue(int value)
        {
            if (this.value != value)
            {
                isDirty = true;
            }
            this.value = value;
            hasValueCache = true;
        }

        public int GetValue()
        {
            return value;
        }

        public void Apply()
        {
            if (isDirty)
            {
                dataProvider.SetInt(key, value);
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