namespace DRG.Core
{
    using Logs;
    using System;
    using System.Collections.Generic;

    public class SignalBus : ISignalBus
    {
        private readonly Dictionary<Type, List<Delegate>> listeners = new Dictionary<Type, List<Delegate>>();
        private readonly ILogger logger;

        private object lockObject = new object();

        private ISignal currentSignal;

        public SignalBus(ILogger logger)
        {
            this.logger = logger;
        }

        public void Subscribe<T>(ISignalBus.SignalHandler<T> callback) where T : ISignal
        {
            lock (lockObject)
            {
                var type = typeof(T);

                if (listeners.TryGetValue(type, out var list))
                {
                    list.Add(callback);
                }
                else
                {
                    listeners[type] = new List<Delegate> { callback };
                }
            }
        }

        public void Unsubscribe<T>(ISignalBus.SignalHandler<T> callback) where T : ISignal
        {
            lock (lockObject)
            {
                var type = typeof(T);

                if (!listeners.TryGetValue(type, out var list))
                {
                    return;
                }

                list.Remove(callback);
            }
        }

        public void Fire<T>(T signal) where T : ISignal
        {
            lock (lockObject)
            {
                if (currentSignal != null)
                {
                    throw new Exception("SignalBus is already processing a signal. Concurrent signal processing is not allowed.");
                }

                var type = typeof(T);

                if (!listeners.TryGetValue(type, out var list))
                {
                    return;
                }

                currentSignal = signal;
                var listenersCopy = new List<Delegate>(list);

                foreach (var listener in listenersCopy)
                {
                    try
                    {
                        var callback = listener as ISignalBus.SignalHandler<T>;
                        callback?.Invoke(signal);
                    }
                    catch (Exception e)
                    {
                        logger.LogException(e);
                    }
                }

                currentSignal = null;
            }
        }

        public void ClearSignalListeners<T>() where T : ISignal
        {
            lock (lockObject)
            {
                var type = typeof(T);
                listeners.Remove(type);
            }
        }

        public void ClearAllListeners()
        {
            lock (lockObject)
            {
                listeners.Clear();
            }
        }

    }
}