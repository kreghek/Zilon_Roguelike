using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial.PathFinding;

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

        /// <summary>
        /// Проверяет, является ли данный узел доступным для текущего актёра.
        /// </summary>
        /// <param name="targetNode"> Целевой узел. </param>
        /// <param name="actor"> Проверяемый актёр. </param>
        /// <returns>true, если указанный узел проходим для актёра. Иначе - false. </returns>
        bool IsPositionAvailableFor(IMapNode targetNode, IActor actor);

        /// <summary>
        /// Проверяет, является ли данный узел доступным для текущего контейнера.
        /// </summary>
        /// <param name="targetNode"> Целевой узел. </param>
        /// <returns>true, если указанный узел подходит для размещения контейнера. Иначе - false. </returns>
        bool IsPositionAvailableForContainer(IMapNode targetNode);

        /// <summary>
        /// Указывает, что узел карты освобождён одним из блоков.
        /// </summary>
        /// <param name="node"> Узел, который будет освобождён указанным блоком. </param>
        /// <param name="blocker"> Блокер, который освобождает узел. </param>
        void ReleaseNode(IMapNode node, IPassMapBlocker blocker);

        /// <summary>
        /// Указывает, что узел карты занят блоком.
        /// </summary>
        /// <param name="node"> Узел, который будет занят указанным блоком. </param>
        /// <param name="blocker"> Блокер, который занимает узел. </param>
        void HoldNode(IMapNode node, IPassMapBlocker blocker);

        /// <summary>
        /// Выполняет поиск пути к указанному узлу.
        /// </summary>
        /// <param name="start"> Начальный узел поиска пути. </param>
        /// <param name="end"> Целевой узел поиска пути. </param>
        /// <param name="context">Контекст поиска пути.</param>
        /// <param name="outputPath">В результате будет содержать набор узлов,
        /// представляющих путь из указанного узла в целевой.</param>
        /// <remarks>
        /// Передача списка для результатов сделана для оптимизации - не нужно каждый раз создавать список
        /// и выделять под него память в зависимости от найденного пути.
        /// </remarks>
        void FindPath(IMapNode start, IMapNode end, PathFindingContext context, List<IMapNode> outputPath);
    }
}