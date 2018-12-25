using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public interface ISectorFactory
    {
        ISector Create(IMap map);
    }
}
