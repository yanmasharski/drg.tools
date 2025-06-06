namespace DRG.Utils
{
    using System;
    using ILogger = Logs.ILogger;
    using UnityEngine;
    using System.Collections;
    
    /// <summary>
    /// Internal MonoBehaviour that handles delayed save operations.
    /// </summary>
    public class DebouncedExecutorUnity : IDebouncedExecutor
    {
        private readonly MonoBehaviour monoBehaviour;
        private readonly ILogger logger;

        public DebouncedExecutorUnity(MonoBehaviour monoBehaviour, ILogger logger)
        {
            this.monoBehaviour = monoBehaviour;
            this.logger = logger;
        }

        public IDebouncedExecutor.ICommand Execute(int cooldown, IEnumerator action)
        {
            var command = new Command(monoBehaviour, logger);
            command.Execute(cooldown, action);
            return command;
        }

        public IDebouncedExecutor.ICommand Execute(int cooldown, Action action)
        {
            var command = new Command(monoBehaviour, logger);
            command.Execute(cooldown, action);
            return command;
        }

        private class Command : IDebouncedExecutor, IDebouncedExecutor.ICommand
        {
            private readonly MonoBehaviour monoBehaviour;
            private readonly ILogger logger;

            private Coroutine executionCoroutine;
            private int framesCooldown;

            public Command(MonoBehaviour monoBehaviour, ILogger logger)
            {
                this.monoBehaviour = monoBehaviour;
                this.logger = logger;
            }

            public bool isRunning => executionCoroutine != null;

            public void Cancel()
            {
                if (executionCoroutine == null)
                {
                    return;
                }

                monoBehaviour.StopCoroutine(executionCoroutine);
                executionCoroutine = null;
            }

            public IDebouncedExecutor.ICommand Execute(int framesCooldown, IEnumerator action)
            {
                if (executionCoroutine != null)
                {
                    monoBehaviour.StopCoroutine(executionCoroutine);
                }

                executionCoroutine = monoBehaviour.StartCoroutine(ExecutionCoroutine(framesCooldown, action));
                return this;
            }

            public IDebouncedExecutor.ICommand Execute(int framesCooldown, Action action)
            {
                if (executionCoroutine != null)
                {
                    monoBehaviour.StopCoroutine(executionCoroutine);
                }

                executionCoroutine = monoBehaviour.StartCoroutine(ExecutionCoroutine(framesCooldown, ExecutionProxy(action)));
                return this;
            }

            /// <summary>
            /// Coroutine that waits for the specified number of frames before trigerring action.
            /// </summary>
            /// <param name="cooldown">Number of frames to wait before triggering action.</param>
            /// <param name="action">Action to be called after delay.</param>
            /// <returns>IEnumerator for the coroutine.</returns>
            private IEnumerator ExecutionCoroutine(int cooldown, IEnumerator action)
            {
                framesCooldown = cooldown;

                while (framesCooldown > 0)
                {
                    yield return null;
                    framesCooldown--;
                }

                yield return action;

                executionCoroutine = null;
            }

            private IEnumerator ExecutionProxy(Action action)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }

                yield break;
            }
        }
    }
}