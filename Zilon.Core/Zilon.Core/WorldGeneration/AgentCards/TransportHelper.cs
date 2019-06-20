using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    /// <summary>
    /// Вспомогательный класс для работы с перемещениями по нас.пунктам.
    /// </summary>
    public static class TransportHelper
    {
        /// <summary>
        /// Выбрасывает целевой населённый пункт для перемещения агента. Нас.пункт выбирается из пунктов государства агента.
        /// </summary>
        /// <param name="localities"> Список всех нас.пунктов. </param>
        /// <param name="dice"> Сервис кубов. </param>
        /// <param name="agent"> Агент, для которого выбирается целевой узел транспортировки. </param>
        /// <param name="currentLocality"> Текущий нас.пункт. </param>
        /// <returns> Возвращает новый населённый пункт или null, если такой невозможно выбрать. </returns>
        /// <remarks>
        /// localities может содержать все нас.пункты - текущий, других государств.
        /// Метод среди них выбирает только тот, который подходит.
        /// Таким является не текущий случайный нас.пункт государсва агента.
        /// </remarks>
        public static Locality RollTargetLocality(List<Locality> localities, IDice dice, Realm agentRealm, Locality currentLocality)
        {
            var count = localities.Count();
            var currentIndex = dice.Roll(0, count - 1);
            var startIndex = currentIndex;
            Locality targetLocality = null;
            while (targetLocality == null)
            {
                var locality = localities[currentIndex];

                currentIndex++;

                // Зацикливаем индекс
                if (currentIndex >= count)
                {
                    currentIndex = 0;
                }

                // Проверяем обход всего набора
                if (startIndex == currentIndex)
                {
                    // Достигли точки, с которой начали обход.
                    // Значит не нашли подходящего агента.

                    break;
                }

                if (locality == currentLocality)
                {
                    continue;
                }

                if (locality.Owner == agentRealm)
                {
                    targetLocality = locality;
                    break;
                }
            }

            return targetLocality;
        }

        /// <summary>
        /// Выполняет перемещение агента в произвольный нас.пункт текущего государства, если это возможно.
        /// </summary>
        /// <param name="globe"> Мир. </param>
        /// <param name="dice"> Сервис случайностей. </param>
        /// <param name="agent"> Агент для перемещения. </param>
        /// <param name="currentLocality"> Текущая позиция агента. </param>
        /// <returns> Возвращает true в случае успешного перемещения. Иначе - false. </returns>
        /// <remarks>
        /// Агент перемещается, если найден подходящий нас. пункт. Иначе остаётся в текущем.
        /// </remarks>
        public static bool TransportAgentToRandomLocality(Globe globe, IDice dice, Agent agent, Locality currentLocality)
        {
            var targetLocality = RollTargetLocality(globe.Localities, dice, agent.Realm, currentLocality);

            if (targetLocality == null)
            {
                return false;
            }

            if (currentLocality != null)
            {
                Helper.RemoveAgentFromCell(globe.AgentCells, agent.Location, agent);
            }

            agent.Location = targetLocality.Cell;

            Helper.AddAgentToCell(globe.AgentCells, agent.Location, agent);

            return true;
        }
    }
}
