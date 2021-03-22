namespace Zilon.Core.World
{
    /// <summary>
    /// Game pool of a persons which try to transit to other sector levels.
    /// </summary>
    public interface ITransitionPool
    {
        /// <summary>
        /// Get a transition info from the pool. Or null if the pool is empty.
        /// </summary>
        /// <returns></returns>
        TransitionPoolItem? Pop();

        /// <summary>
        /// Add information about a person and target sector level.
        /// </summary>
        /// <param name="poolItem"> The pool item is the dat about transition. </param>
        void Push(TransitionPoolItem poolItem);
    }
}