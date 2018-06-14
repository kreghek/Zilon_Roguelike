namespace Zilon.Core.Tactics.Map
{
    using System.Collections.Generic;

    /// <summary>
    /// Тактическая карта.
    /// </summary>
    public interface ICombatMap
    {
        /// <summary>
        /// Список узлов карты.
        /// </summary>
        List<MapNode> Nodes { get; set; }

        /// <summary>
        /// Проверяет, является ли данная ячейка доступной для текущего актёра.
        /// </summary>
        /// <param name="targetNode"> Целевая ячейка. </param>
        /// <param name="actor"> Проверяемый актёр. </param>
        /// <returns></returns>
        bool IsPositionAvailableFor(MapNode targetNode, Actor actor);

        //TODO Проверить необходимость этого метода в интерфейсе.
        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что текущий актёр покинул ячейки.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void ReleaseNode(MapNode node, Actor actor);

        //TODO Проверить необходимость этого метода в интерфейсе.
        // Выглядит, что это внутреняя реализация.
        /// <summary>
        /// Указывает, что данная ячейка занята актёром.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="actor"></param>
        void HoldNode(MapNode node, Actor actor);
    }
}