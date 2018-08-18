using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public interface IMapGenerator
    {
        void CreateMap(IMap map);
    }
}