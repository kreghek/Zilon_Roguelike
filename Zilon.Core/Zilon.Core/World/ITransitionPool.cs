using Zilon.Core.Persons;

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

        /// <summary>
        /// Checks a person in transition pool.
        /// </summary>
        /// <param name="person"> The person to check. </param>
        /// <returns> Returns <c>true</c> if the person is the pool. This means the persin is not in any sector. </returns>
        bool CheckPersonInTransition(IPerson person);
    }
}