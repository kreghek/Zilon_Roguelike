
using Microsoft.Extensions.DependencyInjection;

namespace Zilon.TextClient
{
    internal class GameState
    {
        public GameScreen CurrentScreen { get; set; }
        public ServiceProvider ServiceProvider { get; internal set; }
        public IServiceScope ServiceScope { get; set; }
    }
}