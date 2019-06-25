using System;
using System.Collections.Generic;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Common
{
    public static class ListHelper
    {
        public static T RollRandom<T>(List<T> list, IDice dice, Predicate<T> predicate) where T: class
        {
            var count = list.Count;
            var currentIndex = dice.Roll(0, count - 1);

            var foundIndex = list.FindIndex(currentIndex, predicate);
            if (foundIndex > -1)
            {
                return list[foundIndex];
            }

            if (foundIndex >= 0)
            {
                foundIndex = list.FindIndex(0, currentIndex, predicate);
                return list[foundIndex];
            }
            else
            {
                return null;
            }

            //var startIndex = currentIndex;
            //T foundElement = null;
            //while (foundElement == null)
            //{
            //    var currentElement = list[currentIndex];

            //    currentIndex++;

            //    // Зацикливаем индекс
            //    if (currentIndex >= count)
            //    {
            //        currentIndex = 0;
            //    }

            //    // Проверяем обход всего набора
            //    if (startIndex == currentIndex)
            //    {
            //        // Достигли точки, с которой начали обход.
            //        // Значит не нашли подходящего агента.

            //        break;
            //    }

            //    if (predicate(currentElement))
            //    {
            //        foundElement = currentElement;
            //        break;
            //    }
            //}

            //return foundElement;
        }
    }
}
