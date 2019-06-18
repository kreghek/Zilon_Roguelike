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
        /// <param name="globe"> Мир. </param>
        /// <param name="dice"> Сервис кубов. </param>
        /// <param name="agent"> Агент, для которого выбирается целевой узел транспортировки. </param>
        /// <param name="currentLocality"> Текущий нас.пункт. </param>
        /// <returns> Возвращает новый населённый пункт или null, если такой невозможно выбрать. </returns>
        public static Locality RollTargetLocality(Globe globe, IDice dice, Agent agent, Locality currentLocality)
        {
            var availableTransportLocalities = new List<Locality>();
            foreach (var locality in globe.Localities)
            {
                if (locality == currentLocality)
                {
                    continue;
                }

                if (locality.Owner == agent.Realm)
                {
                    availableTransportLocalities.Add(locality);
                }
            }

            if (!availableTransportLocalities.Any())
            {
                return null;
            }

            var rolledTransportLocalityIndex = dice.Roll(0, availableTransportLocalities.Count() - 1);
            var rolledTransportLocality = availableTransportLocalities[rolledTransportLocalityIndex];

            return rolledTransportLocality;
        }
    }
}
