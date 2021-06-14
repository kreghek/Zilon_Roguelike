using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    /// <summary>
    /// The context of the game loop updates.
    /// </summary>
    public interface IGlobeLoopContext
    {
        /// <summary>
        /// Indicates the game loop has next iteration.
        /// </summary>
        bool HasNextIteration { get; }

        /// <summary>
        /// Update the game loop to next iteration.
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync(CancellationToken cancellationToken);
    }
}