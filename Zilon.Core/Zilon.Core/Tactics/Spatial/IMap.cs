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
        IList<IMapNode> Nodes { get; }

        /// <summary>
        /// Ребра карты.
        /// </summary>
        IList<IEdge> Edges { get; }

        /// <summary>
        /// Регионы карты.
        /// </summary>
        IList<MapRegion> Regions { get; }

        MapRegion StartRegion { get; set; }

        MapRegion ExitRegion { get; set; }

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