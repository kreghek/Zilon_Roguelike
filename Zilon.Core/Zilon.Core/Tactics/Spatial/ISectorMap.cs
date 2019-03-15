using System.Collections.Generic;

using Zilon.Core.MapGenerators.RoomStyle;

namespace Zilon.Core.Tactics.Spatial
{
    public interface ISectorMap: IMap
    {
        Dictionary<IMapNode, RoomTransition> Transitions { get; }

        /// <summary>
        /// Проверяет, доступен ли целевой узел из стартового узла.
        /// </summary>
        /// <param name="currentNode">Стартовый узел.</param>
        /// <param name="targetNode">Целевой проверяемый узел.</param>
        /// <returns> Возвращает true, если узел доступен. Иначе, false.</returns>
        bool TargetIsOnLine(IMapNode currentNode, IMapNode targetNode);

        /// <summary>
        /// Рассчитывает рассточние между двумя узлами карты.
        /// </summary>
        /// <param name="currentNode">Стартовый узел.</param>
        /// <param name="targetNode">Целевой узел.</param>
        /// <returns> Целочисленное значение расстояния. </returns>
        int DistanceBetween(IMapNode currentNode, IMapNode targetNode);
    }
}
