using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Services.CombatMap
{
    public interface IMapGenerator
    {
        void CreateMap(ICombatMap map);
    }
}