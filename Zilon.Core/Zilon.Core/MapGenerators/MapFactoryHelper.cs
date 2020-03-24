using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Schemes;

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
                throw new ArgumentNullException(nameof(sectorScheme));
            }

            if (sectorScheme.TransSectorSids is null)
            {
                return Array.Empty<RoomTransition>();
            }

            return sectorScheme.TransSectorSids.Select(trans => new RoomTransition(trans.SectorLevelSid));
        }
    }
}
