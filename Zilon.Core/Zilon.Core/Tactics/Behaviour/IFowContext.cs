using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Контекст работы с туманом войны.
    /// </summary>
    public interface IFowContext
    {
        /// <summary>
        /// Проверяет, видим ли целевой узел из опорного узла.
        /// </summary>
        /// <param name="baseNode">Опорный узел.</param>
        /// <param name="targetNode">Целевой, проверяемый узел.</param>
        /// <returns></returns>
        bool IsTargetVisible(IGraphNode baseNode, IGraphNode targetNode);

        /// <summary>
        /// Возвращает узлы, соединённые с указанным узлом.
        /// </summary>
        /// <param name="node"> Опорный узел, относительно которого выбираются соседние узлы. </param>
        /// <returns> Возвращает набор соседних узлов. </returns>
        IEnumerable<IGraphNode> GetNext(IGraphNode node);
    }
}
