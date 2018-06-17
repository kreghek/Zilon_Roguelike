namespace Zilon.Core.Tactics.Spatial
{
    using System.Collections.Generic;

    /// <summary>
    /// Тактическая карта.
    /// </summary>
    public interface IMap<TNode, TEdge> where TNode : IMapNode where TEdge : IEdge
    {
        /// <summary>
        /// Список узлов карты.
        /// </summary>
        List<TNode> Nodes { get; set; }

        /// <summary>
        /// Ребра карты.
        /// </summary>
        List<TEdge> Edges { get; set; }

        /// <summary>
        /// Проверяет, является ли данная ячейка доступной для текущего актёра.
        /// </summary>
        /// <param name="targetNode"> Целевая ячейка. </param>
        /// <param name="actor"> Проверяемый актёр. </param>
        /// <returns></returns>
        bool IsPositionAvailableFor(TNode targetNode, Actor actor);

        //TODO Проверить необходимость этого метода в интерфейсе.
        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что текущий актёр покинул ячейки.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void ReleaseNode(TNode node, Actor actor);

        //TODO Проверить необходимость этого метода в интерфейсе.
        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что данная ячейка занята актёром.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void HoldNode(TNode node, Actor actor);
    }
}