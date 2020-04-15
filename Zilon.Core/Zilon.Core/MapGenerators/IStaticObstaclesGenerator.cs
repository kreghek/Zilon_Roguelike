using System.Threading.Tasks;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    public interface IStaticObstaclesGenerator
    {
        Task CreateAsync(ISector sector, ISectorSubScheme sectorSubScheme);
    }
}
