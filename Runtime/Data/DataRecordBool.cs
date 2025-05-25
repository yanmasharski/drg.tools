namespace DRG.Data
{
    public sealed class DataRecordBool : ITypedDataRecord<bool>
    {
        private readonly string keyCache;
        private readonly IDataProvider dataProvider;
        private bool value;
        private bool hasValueCache;

        public DataRecordBool(string key, bool defaultValue, IDataProvider dataProvider)
        {
            keyCache = key;
            this.dataProvider = dataProvider;
            value = dataProvider.GetBool(key, defaultValue);
            hasValueCache = dataProvider.ContainsKey(key);
        }

        public string key => keyCache;
        public bool hasValue => hasValueCache;

        public bool isDirty { get; private set; }

        /// <summary>
        /// Gets whether this record has been fully processed during save operations.
        /// Primarily used for object serialization tracking.
        /// Always returns true for bool records.
        /// </summary>
        public bool processed => true;

        public void SetValue(bool newValue)
        {
            if (value != newValue)
            {
                isDirty = true;
            }
            value = newValue;
            hasValueCache = true;
        }

        public bool GetValue()
        {
            return value;
        }

        public void Apply()
        {
            if (!isDirty)
            {
                return;
            }

            dataProvider.SetBool(key, value);
            isDirty = false;
        }

        public void Erase()
        {
            dataProvider.DeleteKey(key);
            hasValueCache = false;
        }
    }
}
