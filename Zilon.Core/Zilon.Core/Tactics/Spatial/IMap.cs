namespace Zilon.Core.Tactics.Spatial
{
    using System.Collections.Generic;

    /// <summary>
    /// Тактическая карта.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// Список узлов карты.
        /// </summary>
        List<IMapNode> Nodes { get; }

        /// <summary>
        /// Ребра карты.
        /// </summary>
        List<IEdge> Edges { get; }

        /// <summary>
        /// Проверяет, является ли данная ячейка доступной для текущего актёра.
        /// </summary>
        /// <param name="targetNode"> Целевая ячейка. </param>
        /// <param name="actor"> Проверяемый актёр. </param>
        /// <returns></returns>
        bool IsPositionAvailableFor(IMapNode targetNode, Actor actor);

        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что текущий актёр покинул ячейки.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void ReleaseNode(IMapNode node, Actor actor);

        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что данная ячейка занята актёром.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void HoldNode(IMapNode node, Actor actor);
    }
}