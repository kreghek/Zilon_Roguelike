using System.Threading.Tasks;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public interface IBiomeInitializer
    {
        Task<Biome> InitBiomeAsync(ILocationScheme locationScheme);
        Task MaterializeLevel(SectorNode sectorNode);
    }
}