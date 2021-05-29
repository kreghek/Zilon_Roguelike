using System.Threading.Tasks;

namespace Zilon.Emulation.Common
{
    /// <summary>
    /// The context to handle autoplay state.
    /// </summary>
    public interface IAutoplayContext
    {
        /// <summary>
        /// Checks next iteration. If there is no next iteration then autoplay engine stops.
        /// </summary>
        Task<bool> CheckNextIterationAsync();
    }
}