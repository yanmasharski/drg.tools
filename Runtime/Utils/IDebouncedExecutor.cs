namespace DRG.Utils
{
    using System;
    using System.Collections;

    /// <summary>
    /// Interface for executing actions with a cooldown period between executions.
    /// Provides debouncing functionality to prevent actions from being called too frequently.
    /// </summary>
    public interface IDebouncedExecutor
    {
        /// <summary>
        /// Executes a coroutine action with the specified frame cooldown period.
        /// </summary>
        /// <param name="framesCooldown">Number of frames to wait before allowing another execution.</param>
        /// <param name="action">The coroutine action to execute.</param>
        /// <returns>A command object to control and monitor the execution.</returns>
        ICommand Execute(int framesCooldown, IEnumerator action);

        /// <summary>
        /// Executes a simple action with the specified frame cooldown period.
        /// </summary>
        /// <param name="framesCooldown">Number of frames to wait before allowing another execution.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>A command object to control and monitor the execution.</returns>
        ICommand Execute(int framesCooldown, Action action);

        /// <summary>
        /// Interface for controlling and monitoring the execution of a debounced action.
        /// </summary>
        public interface ICommand
        {
            /// <summary>
            /// Gets whether the command is currently executing or in its cooldown period.
            /// </summary>
            bool isRunning { get; }

            /// <summary>
            /// Cancels the current execution and cooldown period.
            /// </summary>
            void Cancel();
        }
    }
}