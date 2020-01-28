using System.Collections.Generic;

namespace Zilon.Core.Graphs
{
    /// <summary>
    /// Граф.
    /// </summary>
    /// <remarks>
    /// Это абстракция графа, как совокупности узлов и рёбер, соединяющих его.
    /// </remarks>
    public interface IGraph
    {
        /// <summary>
        /// Список узлов карты.
        /// </summary>
        IEnumerable<IGraphNode> Nodes { get; }

        /// <summary>
        /// Возвращает узлы, соединённые с указанным узлом.
        /// </summary>
        /// <param name="node"> Опорный узел, относительно которого выбираются соседние узлы. </param>
        /// <returns> Возвращает набор соседних узлов. </returns>
        IEnumerable<IGraphNode> GetNext(IGraphNode node);

        /// <summary>
        /// Добавляет новый узел графа.
        /// </summary>
        /// <param name="node"></param>
        void AddNode(IGraphNode node);

        /// <summary>
        /// Удаляет узел графа.
        /// </summary>
        /// <param name="node"></param>
        void RemoveNode(IGraphNode node);

        /// <summary>
        /// Создаёт ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1"> Узел графа карты. </param>
        /// <param name="node2"> Узел графа карты. </param>
        void AddEdge(IGraphEdge node1, IGraphEdge node2);

        /// <summary>
        /// Удаляет ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1"> Узел графа карты. </param>
        /// <param name="node2"> Узел графа карты. </param>
        void RemoveEdge(IGraphEdge node1, IGraphEdge node2);
    }
}
