using System.Collections.Generic;

namespace Zilon.Core.Graphs
{
    /// <summary>
    /// Граф.
    /// </summary>
    /// <remarks>
    /// Это абстракция графа, как совокупности узлов и рёбер, соединяющих его.
    /// </remarks>
    public interface IGraph<TNode, TEdge> where TNode: IGraphNode where TEdge: IGraphEdge
    {
        /// <summary>
        /// Список узлов карты.
        /// </summary>
        IEnumerable<TNode> Nodes { get; }

        /// <summary>
        /// Возвращает узлы, соединённые с указанным узлом.
        /// </summary>
        /// <param name="node"> Опорный узел, относительно которого выбираются соседние узлы. </param>
        /// <returns> Возвращает набор соседних узлов. </returns>
        IEnumerable<TNode> GetNext(TNode node);

        /// <summary>
        /// Добавляет новый узел графа.
        /// </summary>
        /// <param name="node"></param>
        void AddNode(TNode node);

        /// <summary>
        /// Удаляет узел графа.
        /// </summary>
        /// <param name="node"></param>
        void RemoveNode(TNode node);

        /// <summary>
        /// Создаёт ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1"> Узел графа карты. </param>
        /// <param name="node2"> Узел графа карты. </param>
        void AddEdge(TEdge node1, TEdge node2);

        /// <summary>
        /// Удаляет ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1"> Узел графа карты. </param>
        /// <param name="node2"> Узел графа карты. </param>
        void RemoveEdge(TEdge node1, TEdge node2);
    }
}
