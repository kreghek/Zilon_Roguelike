using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Общий вспомогательный класс для фабрик карты.
    /// </summary>
    public static class MapFactoryHelper
    {
        /// <summary>
        /// Создание переходов на основе схемы.
        /// </summary>
        /// <param name="sectorScheme"> Схема сектора. </param>
        /// <returns> Набор объектов переходов. </returns>
        public static IEnumerable<RoomTransition> CreateTransitions(ISectorSubScheme sectorScheme)
        {
            if (sectorScheme is null)
            {
                throw new System.ArgumentNullException(nameof(sectorScheme));
            }

            return sectorScheme.TransSectorSids.Select(sid => new RoomTransition(sid));
        }
    }
}
