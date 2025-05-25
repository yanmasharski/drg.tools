namespace DRG.Utils
{
    using System.Collections;

    public interface IDebouncedExecutor
    {
        void Execute(int framesCooldown, IEnumerator action);
    }
}