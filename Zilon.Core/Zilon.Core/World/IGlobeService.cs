using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public interface IGlobeService
    {
        Task ExpandGlobeAsync(IGlobe globe, ISectorNode sectorNode);
    }
}