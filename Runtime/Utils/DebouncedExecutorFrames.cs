namespace DRG.Utils
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Internal MonoBehaviour that handles delayed save operations.
    /// </summary>
    public class DebouncedExecutorFrames : MonoBehaviour, IDebouncedExecutor
    {
        private Coroutine saveCoroutine;
        private int framesCooldown;

        public static DebouncedExecutorFrames current { get; private set; }
        
        public static void Init()
        {
            current = new GameObject("DebouncedExecutorFrames").AddComponent<DebouncedExecutorFrames>();
            GameObject.DontDestroyOnLoad(current.gameObject);
        }

        public void Execute(int framesCooldown, IEnumerator action)
        {
            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
            }

            saveCoroutine = StartCoroutine(SaveCoroutine(framesCooldown, action));
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