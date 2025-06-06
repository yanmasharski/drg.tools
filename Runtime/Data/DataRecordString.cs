namespace DRG.Data
{
    using DRG.Logs;

    public sealed class DataRecordString : ITypedDataRecord<string>
    {
        private readonly string keyCache;
        private readonly IDataProvider dataProvider;
        private readonly ILogger logger;
        private string value;
        private bool hasValueCache;

        public DataRecordString(string key, string defaultValue, IDataProvider dataProvider, ILogger logger)
        {
            keyCache = key;
            this.dataProvider = dataProvider;
            value = dataProvider.GetString(key, defaultValue);
            hasValueCache = dataProvider.ContainsKey(key);
            this.logger = logger;
        }
        public string key => keyCache;
        public bool hasValue => hasValueCache;

        public bool isDirty { get; private set; }

        /// <summary>
        /// Gets whether this record has been fully processed during save operations.
        /// Primarily used for object serialization tracking.
        /// Always returns true for string records.
        /// </summary>
        public bool processed => true;

        public void SetValue(string value)
        {
            if (this.value != value)
            {
                isDirty = true;
            }
            this.value = value;
            hasValueCache = true;
        }

        public string GetValue()
        {
            return value;
        }

        public void Apply()
        {
            if (isDirty)
            {
                dataProvider.SetString(key, value);
                isDirty = false;

                if (value.Length > 20000)
                {
                    logger.LogWarning($"Potential ANR warning! String with length {value.Length} is too large for {key}.");
                }
            }
        }

        public void Erase()
        {
            dataProvider.DeleteKey(key);
            hasValueCache = false;
        }
    }
}
