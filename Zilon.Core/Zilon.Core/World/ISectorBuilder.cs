using System.Threading.Tasks;

using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public interface ISectorBuilder
    {
        Task<ISector> CreateSectorAsync();
    }
}
