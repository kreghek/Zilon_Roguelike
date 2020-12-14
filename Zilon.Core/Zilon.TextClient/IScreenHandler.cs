using System.Threading.Tasks;

namespace Zilon.TextClient
{
    internal interface IScreenHandler
    {
        Task<GameScreen> StartProcessingAsync(GameState gameState);
    }
}