using System.Collections.Generic;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Интерфейс для генератора монстров.
    /// </summary>
    /// <remarks>
    /// Сейчас используется процедурным генератором сектора.
    /// </remarks>
    public interface IMonsterGenerator
    {
        void CreateMonsters(ISector sector,
            IEnumerable<MapRegion> monsterRegions,
            IMonsterGeneratorOptions monsterGeneratorOptions);
    }
}
