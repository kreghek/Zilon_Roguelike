using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial
{
    public interface ISectorMap : IMap
    {
        Dictionary<IGraphNode, SectorTransition> Transitions { get; }

        /// <summary>
        /// Проверяет, доступен ли целевой узел из стартового узла.
        /// </summary>
        /// <param name="currentNode">Стартовый узел.</param>
        /// <param name="targetNode">Целевой проверяемый узел.</param>
        /// <returns> Возвращает true, если узел доступен. Иначе, false.</returns>
        bool TargetIsOnLine(IGraphNode currentNode, IGraphNode targetNode);
    }
}