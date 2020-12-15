using System.Threading.Tasks;

namespace Zilon.TextClient
{
    /// <summary>
    /// Screen handler interface to work with current game state.
    /// Screen == game state.
    /// </summary>
    internal interface IScreenHandler
    {
        /// <summary>
        /// Processing game state while next screen will be transited.
        /// </summary>
        /// <param name="gameState"> Inner common game data. </param>
        /// <returns> Identifier of next game screen. </returns>
        Task<GameScreen> StartProcessingAsync(GameState gameState);
    }
}