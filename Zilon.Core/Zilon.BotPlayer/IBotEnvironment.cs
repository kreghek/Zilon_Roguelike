using System.Threading.Tasks;

namespace Zilon.BotPlayer
{
    public interface IBotEnvironment
    {
        Task<string> RequestAsync(string requestText);
    }
}
