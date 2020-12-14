using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Zilon.TextClient
{
    internal interface IScreenHandler
    {
        Task<GameScreen> StartProcessingAsync(IServiceScope serviceScope);
    }
}