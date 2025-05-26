namespace DRG.Utils
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Internal MonoBehaviour that handles delayed save operations.
    /// </summary>
    public class DebouncedExecutorUnity : IDebouncedExecutor
    {
        private readonly MonoBehaviour monoBehaviour;
        private Coroutine saveCoroutine;
        private int framesCooldown;

        public DebouncedExecutorUnity(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
        }

        public void Execute(int framesCooldown, IEnumerator action)
        {
            if (saveCoroutine != null)
            {
                monoBehaviour.StopCoroutine(saveCoroutine);
            }

            saveCoroutine = monoBehaviour.StartCoroutine(SaveCoroutine(framesCooldown, action));
        }

        /// <summary>
        /// Coroutine that waits for the specified number of frames before saving.
        /// </summary>
        /// <param name="framesCooldown">Number of frames to wait before saving.</param>
        /// <returns>IEnumerator for the coroutine.</returns>
        public IEnumerator SaveCoroutine(int framesCooldown, IEnumerator processAndSave)
        {
            this.framesCooldown = framesCooldown;

            while (this.framesCooldown > 0)
            {
                yield return null;
                this.framesCooldown--;
            }

            yield return processAndSave;

            saveCoroutine = null;
        }
    }
}