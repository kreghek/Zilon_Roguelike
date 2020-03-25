using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Schemes;
using Zilon.Core.World;

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
        /// <param name="sectorNode"> Схема сектора. </param>
        /// <returns> Набор объектов переходов. </returns>
        public static IEnumerable<RoomTransition> CreateTransitions(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            if (sectorNode.State != SectorNodeState.SectorMaterialized)
            {
                throw new ArgumentException("Узел сектора должен быть материализован", nameof(sectorNode));
            }

            var next = sectorNode.Biome.GetNext(sectorNode);

            return next.Select(node => new RoomTransition(node as ISectorNode));
        }
    }
}
