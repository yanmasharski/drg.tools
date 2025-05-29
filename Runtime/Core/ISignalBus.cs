namespace DRG.Core
{
    /// <summary>
    /// Interface for a signal bus that can be used to communicate between different parts of the application.
    /// Provides methods for subscribing to, unsubscribing from, and firing signals.
    /// </summary>
    public interface ISignalBus
    {
        /// <summary>
        /// Subscribes a callback to receive signals of type T.
        /// </summary>
        /// <typeparam name="T">The type of signal to subscribe to.</typeparam>
        /// <param name="callback">The callback method that will be invoked when the signal is fired.</param>
        void Subscribe<T>(SignalHandler<T> callback) where T : ISignal;

        /// <summary>
        /// Unsubscribes a callback from receiving signals of type T.
        /// </summary>
        /// <typeparam name="T">The type of signal to unsubscribe from.</typeparam>
        /// <param name="callback">The callback method to unsubscribe.</param>
        void Unsubscribe<T>(SignalHandler<T> callback) where T : ISignal;

        /// <summary>
        /// Fires a signal, invoking all subscribed callbacks for that signal type.
        /// </summary>
        /// <typeparam name="T">The type of signal being fired.</typeparam>
        /// <param name="signal">The signal instance to fire.</param>
        void Fire<T>(T signal) where T : ISignal;

        /// <summary>
        /// Removes all listeners for a specific signal type.
        /// </summary>
        /// <typeparam name="T">The type of signal to clear listeners for.</typeparam>
        void ClearSignalListeners<T>() where T : ISignal;

        /// <summary>
        /// Removes all signal listeners for all signal types.
        /// </summary>
        void ClearAllListeners();

        /// <summary>
        /// Delegate for signal handler callbacks.
        /// </summary>
        /// <typeparam name="T">The type of signal this handler processes.</typeparam>
        /// <param name="signal">The signal instance being handled.</param>
        public delegate void SignalHandler<T>(T signal) where T : ISignal;
    }
}