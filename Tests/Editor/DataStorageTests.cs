namespace DRG.Tests
{
    using NUnit.Framework;
    using DRG.Data;
    using DRG.Utils;
    using DRG.Serialization;
    using DRG.Logs;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.TestTools;
    using ILogger = DRG.Logs.ILogger;

    [TestFixture]
    public class DataStorageTests
    {
        private IDataProvider mockDataProvider;
        private ILogger mockLogger;
        private IDebouncedExecutor mockDebouncedExecutor;
        private DataStorage dataStorage;

        [SetUp]
        public void Setup()
        {
            mockDataProvider = new MockDataProvider();
            mockLogger = new MockLogger();
            mockDebouncedExecutor = new MockDebouncedExecutor();
            DataStorage.Init(mockDataProvider, mockLogger, mockDebouncedExecutor);
            dataStorage = DataStorage.current;
        }

        [TearDown]
        public void TearDown()
        {
            dataStorage.EraseAll();
        }

        [Test]
        public void GetInt_WithNewKey_ReturnsDefaultValue()
        {
            // Arrange
            const string key = "test_int";
            const int defaultValue = 42;

            // Act
            var record = dataStorage.GetInt(key, defaultValue);

            // Assert
            Assert.That(record.GetValue(), Is.EqualTo(defaultValue));
            Assert.That(record.hasValue, Is.False);
        }

        [Test]
        public void GetInt_WithExistingKey_ReturnsStoredValue()
        {
            // Arrange
            const string key = "test_int";
            const int value = 42;
            var record = dataStorage.GetInt(key);
            record.SetValue(value);
            record.Apply();

            // Act
            var retrievedRecord = dataStorage.GetInt(key);

            // Assert
            Assert.That(retrievedRecord.GetValue(), Is.EqualTo(value));
            Assert.That(retrievedRecord.hasValue, Is.True);
        }

        [Test]
        public void GetBool_WithNewKey_ReturnsDefaultValue()
        {
            // Arrange
            const string key = "test_bool";
            const bool defaultValue = true;

            // Act
            var record = dataStorage.GetBool(key, defaultValue);

            // Assert
            Assert.That(record.GetValue(), Is.EqualTo(defaultValue));
            Assert.That(record.hasValue, Is.False);
        }

        [Test]
        public void GetFloat_WithNewKey_ReturnsDefaultValue()
        {
            // Arrange
            const string key = "test_float";
            const float defaultValue = 3.14f;

            // Act
            var record = dataStorage.GetFloat(key, defaultValue);

            // Assert
            Assert.That(record.GetValue(), Is.EqualTo(defaultValue));
            Assert.That(record.hasValue, Is.False);
        }

        [Test]
        public void GetString_WithNewKey_ReturnsDefaultValue()
        {
            // Arrange
            const string key = "test_string";
            const string defaultValue = "default";

            // Act
            var record = dataStorage.GetString(key, defaultValue);

            // Assert
            Assert.That(record.GetValue(), Is.EqualTo(defaultValue));
            Assert.That(record.hasValue, Is.False);
        }

        [Test]
        public void GetObject_WithNewKey_ReturnsDefaultValue()
        {
            // Arrange
            const string key = "test_object";
            var defaultValue = new TestObject { Value = 42 };
            var serializer = new DataSerializerUnity();

            // Act
            var record = dataStorage.GetObject(key, defaultValue, serializer);

            // Assert
            Assert.That(record.GetValue().Value, Is.EqualTo(defaultValue.Value));
            Assert.That(record.hasValue, Is.False);
        }

        [Test]
        public void ContainsKey_WithNonExistentKey_ReturnsFalse()
        {
            // Arrange
            const string key = "non_existent_key";

            // Act
            var result = dataStorage.ContainsKey(key);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsKey_WithExistingKey_ReturnsTrue()
        {
            // Arrange
            const string key = "test_key";
            var record = dataStorage.GetInt(key);
            record.SetValue(42);
            record.Apply();

            // Act
            var result = dataStorage.ContainsKey(key);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Erase_WithExistingKey_RemovesKey()
        {
            // Arrange
            const string key = "test_key";
            var record = dataStorage.GetInt(key);
            record.SetValue(42);
            record.Apply();

            // Act
            dataStorage.Erase(key);

            // Assert
            Assert.That(dataStorage.ContainsKey(key), Is.False);
        }

        [Test]
        public void EraseAll_WithMultipleKeys_RemovesAllKeys()
        {
            // Arrange
            var intRecord = dataStorage.GetInt("test_int");
            var boolRecord = dataStorage.GetBool("test_bool");
            var floatRecord = dataStorage.GetFloat("test_float");
            var stringRecord = dataStorage.GetString("test_string");

            intRecord.SetValue(42);
            boolRecord.SetValue(true);
            floatRecord.SetValue(3.14f);
            stringRecord.SetValue("test");

            intRecord.Apply();
            boolRecord.Apply();
            floatRecord.Apply();
            stringRecord.Apply();

            // Act
            dataStorage.EraseAll();

            // Assert
            Assert.That(dataStorage.ContainsKey("test_int"), Is.False);
            Assert.That(dataStorage.ContainsKey("test_bool"), Is.False);
            Assert.That(dataStorage.ContainsKey("test_float"), Is.False);
            Assert.That(dataStorage.ContainsKey("test_string"), Is.False);
        }

        [Test]
        public void Save_WithDirtyRecords_SavesChanges()
        {
            // Arrange
            const string key = "test_key";
            var record = dataStorage.GetInt(key);
            record.SetValue(42);

            // Act
            dataStorage.Save(0);

            // Assert
            var retrievedRecord = dataStorage.GetInt(key);
            Assert.That(retrievedRecord.GetValue(), Is.EqualTo(42));
            Assert.That(retrievedRecord.isDirty, Is.False);
        }

        [Serializable]
        private class TestObject
        {
            public int Value;
        }

        private class MockDataProvider : IDataProvider
        {
            private readonly System.Collections.Generic.Dictionary<string, object> storage = new System.Collections.Generic.Dictionary<string, object>();

            public int GetInt(string key, int defaultValue)
            {
                return storage.TryGetValue(key, out var value) ? (int)value : defaultValue;
            }

            public float GetFloat(string key, float defaultValue)
            {
                return storage.TryGetValue(key, out var value) ? (float)value : defaultValue;
            }

            public string GetString(string key, string defaultValue)
            {
                return storage.TryGetValue(key, out var value) ? (string)value : defaultValue;
            }

            public bool GetBool(string key, bool defaultValue)
            {
                return storage.TryGetValue(key, out var value) ? (bool)value : defaultValue;
            }

            public void SetInt(string key, int value)
            {
                storage[key] = value;
            }

            public void SetFloat(string key, float value)
            {
                storage[key] = value;
            }

            public void SetString(string key, string value)
            {
                storage[key] = value;
            }

            public void SetBool(string key, bool value)
            {
                storage[key] = value;
            }

            public bool ContainsKey(string key)
            {
                return storage.ContainsKey(key);
            }

            public void DeleteKey(string key)
            {
                storage.Remove(key);
            }

            public void DeleteAll()
            {
                storage.Clear();
            }

            public void Save()
            {
                // No-op for mock
            }
        }

        private class MockLogger : ILogger
        {
            public void Log(string message)
            {
                Debug.Log(message);
            }

            public void LogWarning(string message)
            {
                Debug.LogWarning(message);
            }

            public void LogError(string message)
            {
                Debug.Log(message);
            }

            public void LogException(Exception exception)
            {
                Debug.Log(exception.Message);
            }
        }

        private class MockDebouncedExecutor : IDebouncedExecutor
        {
            public void Execute(int framesCooldown, IEnumerator action)
            {
                // Execute immediately for testing
                while (action.MoveNext()) { }
            }
        }
    }
}

