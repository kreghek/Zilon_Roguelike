using System.Threading.Tasks;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public interface IBiomInitializer
    {
        Task<Biom> InitBiomAsync(ILocationScheme locationScheme);
        Task MaterializeLevel(SectorNode sectorNode);
    }
}