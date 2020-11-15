using Zilon.Core.Graphs;
using Zilon.Core.PathFinding;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Карта, как граф.
    /// </summary>
    public interface IMap : ISpatialGraph
    {
        /// <summary>
        /// Регионы карты.
        /// </summary>
        IList<MapRegion> Regions { get; }

        /// <summary>
        /// Выполняет поиск пути к указанному узлу.
        /// </summary>
        /// <param name="startNode"> Начальный узел поиска пути. </param>
        /// <param name="targetNode"> Целевой узел поиска пути. </param>
        /// <param name="context">Контекст поиска пути.</param>
        /// <param name="outputPath">В результате будет содержать набор узлов,
        /// представляющих путь из указанного узла в целевой.</param>
        /// <remarks>
        /// Передача списка для результатов сделана для оптимизации - не нужно каждый раз создавать список
        /// и выделять под него память в зависимости от найденного пути.
        /// </remarks>
        void FindPath(
            IGraphNode startNode,
            IGraphNode targetNode,
            IAstarContext context,
            List<IGraphNode> outputPath);

        /// <summary>
        /// Указывает, что узел карты занят блоком.
        /// </summary>
        /// <param name="node"> Узел, который будет занят указанным блоком. </param>
        /// <param name="blocker"> Блокер, который занимает узел. </param>
        void HoldNode(IGraphNode node, IPassMapBlocker blocker);

        /// <summary>
        /// Проверяет, является ли данный узел доступным для текущего актёра.
        /// </summary>
        /// <param name="targetNode"> Целевой узел. </param>
        /// <param name="actor"> Проверяемый актёр. </param>
        /// <returns>true, если указанный узел проходим для актёра. Иначе - false. </returns>
        bool IsPositionAvailableFor(IGraphNode targetNode, IActor actor);

        /// <summary>
        /// Проверяет, является ли данный узел доступным для текущего контейнера.
        /// </summary>
        /// <param name="targetNode"> Целевой узел. </param>
        /// <returns>true, если указанный узел подходит для размещения контейнера. Иначе - false. </returns>
        bool IsPositionAvailableForContainer(IGraphNode targetNode);

        /// <summary>
        /// Указывает, что узел карты освобождён одним из блоков.
        /// </summary>
        /// <param name="node"> Узел, который будет освобождён указанным блоком. </param>
        /// <param name="blocker"> Блокер, который освобождает узел. </param>
        void ReleaseNode(IGraphNode node, IPassMapBlocker blocker);
    }
}