using System.Collections.Generic;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Тактическая карта.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// Список узлов карты.
        /// </summary>
        IEnumerable<IMapNode> Nodes { get; }

        /// <summary>
        /// Возвращает узлы, напрямую соединённые с указанным узлом.
        /// </summary>
        /// <param name="node"> Опорный узел, относительно которого выбираются соседние узлы. </param>
        /// <returns> Возвращает набор соседних узлов. </returns>
        IEnumerable<IMapNode> GetNext(IMapNode node);

        /// <summary>
        /// Добавляет новый узел графа.
        /// </summary>
        /// <param name="node"></param>
        void AddNode(IMapNode node);

        /// <summary>
        /// Создаёт ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1"> Узел графа карты. </param>
        /// <param name="node2"> Узел графа карты. </param>
        void AddEdge(IMapNode node1, IMapNode node2);

        /// <summary>
        /// Удаляет ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1"> Узел графа карты. </param>
        /// <param name="node2"> Узел графа карты. </param>
        void RemoveEdge(IMapNode node1, IMapNode node2);

        /// <summary>
        /// Регионы карты.
        /// </summary>
        IList<MapRegion> Regions { get; }

        MapRegion StartRegion { get; set; }

        IMapNode[] StartNodes { get; set; }

        MapRegion ExitRegion { get; set; }

        IMapNode[] ExitNodes { get; set; }

        /// <summary>
        /// Проверяет, является ли данная ячейка доступной для текущего актёра.
        /// </summary>
        /// <param name="targetNode"> Целевая ячейка. </param>
        /// <param name="actor"> Проверяемый актёр. </param>
        /// <returns>true, если указанный узел проходим для актёра. Иначе - false. </returns>
        bool IsPositionAvailableFor(IMapNode targetNode, IActor actor);

        //TODO Выглядит, что это внутреняя реализация. (Чего?)
        /// <summary>
        /// Указывает, что узел карты освобождён одним из блоков.
        /// </summary>
        /// <param name="node"> Узел, который будет освобождён указанным блоком. </param>
        /// <param name="blocker"> Блокер, который освобождает узел. </param>
        void ReleaseNode(IMapNode node, IPassMapBlocker blocker);

        //TODO Выглядит, что это внутреняя реализация. (Чего? Какой сущности?)
        /// <summary>
        /// Указывает, что узел карты занят блоком.
        /// </summary>
        /// <param name="node"> Узел, который будет занят указанным блоком. </param>
        /// <param name="blocker"> Блокер, который занимает узел. </param>
        void HoldNode(IMapNode node, IPassMapBlocker blocker);
    }
}