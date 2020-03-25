using System.Threading.Tasks;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public interface IBiomeInitializer
    {
        Task<IBiome> InitBiomeAsync(ILocationScheme locationScheme);
        Task MaterializeLevel(ISectorNode sectorNode);
    }
}