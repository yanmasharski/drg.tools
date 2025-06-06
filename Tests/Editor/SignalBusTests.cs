using NUnit.Framework;
using DRG.Core;
using DRG.Logs;

namespace DRG.Tests
{
    public class SignalBusTests
    {
        private class MockLogger : ILogger
        {
            public void Log(string message) { }
            public void LogWarning(string message) { }
            public void LogError(string message) { }
            public void LogException(System.Exception exception) { }
        }

        private class TestSignal : ISignal { }

        [Test]
        public void SubscribeAndFire_InvokesCallback()
        {
            var logger = new MockLogger();
            var bus = new SignalBus(logger);
            int callCount = 0;

            bus.Subscribe<TestSignal>(_ => callCount++);
            bus.Fire(new TestSignal());

            Assert.That(callCount, Is.EqualTo(1));
        }

        [Test]
        public void Unsubscribe_RemovesCallback()
        {
            var logger = new MockLogger();
            var bus = new SignalBus(logger);
            int callCount = 0;
            ISignalBus.SignalHandler<TestSignal> handler = _ => callCount++;

            bus.Subscribe(handler);
            bus.Unsubscribe(handler);
            bus.Fire(new TestSignal());

            Assert.That(callCount, Is.EqualTo(0));
        }
    }
}
