using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    /// <summary>
    /// Service to work with animation blocker. Blockers required to corrently animate actions of visible actors.
    /// </summary>
    public interface IAnimationBlockerService
    {
        /// <summary>
        /// Check there are any blockers was added but not released.
        /// </summary>
        bool HasBlockers { get; }

        /// <summary>
        /// Add animation blocker.
        /// </summary>
        /// <param name="commandBlocker"> Blocker created by game object which start some animation. </param>
        void AddBlocker(ICommandBlocker commandBlocker);

        /// <summary>
        /// Delete all blocker forced.
        /// </summary>
        void DropBlockers();

        /// <summary>
        /// Waits until all animation blocker will be released in async manner.
        /// </summary>
        Task WaitBlockersAsync();
    }
}