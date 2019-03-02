using System.Collections.Generic;

using Zilon.Core.Players;
using Zilon.Core.Schemes;
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
        /// <summary> Создаёт монстров в секторе по указанной схеме. </summary>
        /// <param name="sector"> Целевой сектор. </param>
        /// <param name="monsterPlayer"> Бот, управляющий монстрами. По сути, команда монстров. </param>
        /// <param name="monsterRegions"> Регионы сектора, где могут быть монстры. </param>
        /// <param name="sectorScheme"> Схема сектора. Отсюда берутся параметры генерации монстров. </param>
        void CreateMonsters(ISector sector,
            IBotPlayer monsterPlayer,
            IEnumerable<MapRegion> monsterRegions,
            ISectorSubScheme sectorScheme);
    }
}
