using System;
using System.Collections.Generic;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Common
{
    public static class ListHelper
    {
        public static T RollRandom<T>(IList<T> list, IDice dice, Predicate<T> predicate) where T: class
        {
            var count = list.Count;
            var currentIndex = dice.Roll(0, count - 1);
            var startIndex = currentIndex;
            T foundElement = null;
            while (foundElement == null)
            {
                var currentElement = list[currentIndex];

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

                if (predicate(currentElement))
                {
                    foundElement = currentElement;
                    break;
                }
            }

            return foundElement;
        }
    }
}
