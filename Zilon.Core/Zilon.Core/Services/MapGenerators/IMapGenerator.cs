using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Services.CombatMap
{
    public interface IMapGenerator
    {
        void CreateMap(IHexMap map);
    }
}