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
        List<HexNode> Nodes { get; set; }

        /// <summary>
        /// Проверяет, является ли данная ячейка доступной для текущего актёра.
        /// </summary>
        /// <param name="targetNode"> Целевая ячейка. </param>
        /// <param name="actor"> Проверяемый актёр. </param>
        /// <returns></returns>
        bool IsPositionAvailableFor(HexNode targetNode, Actor actor);

        //TODO Проверить необходимость этого метода в интерфейсе.
        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что текущий актёр покинул ячейки.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void ReleaseNode(HexNode node, Actor actor);

        //TODO Проверить необходимость этого метода в интерфейсе.
        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что данная ячейка занята актёром.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void HoldNode(HexNode node, Actor actor);
    }
}