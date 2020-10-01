using System.Collections.Generic;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Вспомогательный класс для определения переходов.
    /// </summary>
    public static class TransitionDetection
    {
        /// <summary> Определяет, что все указанные узлы (в них обычно стоят актёры игрока) находятся в одном узле перехода. </summary>
        /// <param name="transitions"> Доступные переходы. </param>
        /// <param name="actorNodes"> Набор проверяемых узлов. Сюда передаются узлы актеров, которые пренадлежат игроку. </param>
        /// <returns> Возвращает отработавший переход. Иначе возаращает null. </returns>
        public static RoomTransition Detect(IDictionary<IGraphNode, RoomTransition> transitions, IEnumerable<IGraphNode> actorNodes)
        {
            // Из сектора нет прямого выхода.
            // Нужно для упрощения тестов сектора или карты.
            // Дальше будут специальные переходы. Например, в результате диалога.
            if (transitions == null)
            {
                return null;
            }

            var allExit = true;

            //Проверяем, что есть хоть один персонаж игрока.
            // Потому что, умирая, персонажи удаляются из менеджера.
            // И проверка сообщает, что нет ниодного персонажа игрока вне узлов выхода.
            var atLeastOneHuman = false;

            RoomTransition expectedTransition = null;
            foreach (var actorNode in actorNodes)
            {
                atLeastOneHuman = true;

                if (!transitions.TryGetValue(actorNode, out var transition))
                {
                    continue;
                }

                if (expectedTransition == null)
                {
                    expectedTransition = transition;
                }
                else if (expectedTransition != transition)
                {
                    allExit = false;
                    break;
                }
            }

            if (atLeastOneHuman && allExit)
            {
                return expectedTransition;
            }

            // означает, что переход не задействован
            return null;
        }
    }
}
