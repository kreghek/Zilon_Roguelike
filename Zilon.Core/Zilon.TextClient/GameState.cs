using Microsoft.Extensions.DependencyInjection;

namespace Zilon.TextClient
{
    /// <summary>
    /// The common system text game data.
    /// </summary>
    internal class GameState
    {
        /// <summary>
        /// Current game screen.
        /// </summary>
        public GameScreen CurrentScreen { get; set; }

        /// <summary>
        /// Full service provider.
        /// </summary>
        public ServiceProvider ServiceProvider { get; internal set; }

        /// <summary>
        /// Service provider scope for current game state.
        /// </summary>
        public IServiceScope ServiceScope { get; set; }
    }
}