using System;
using System.Collections.Generic;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Common
{
    public static class ListHelper
    {
        public static T RollRandom<T>(List<T> list, IDice dice, Predicate<T> predicate) where T : class
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

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
        }
    }
}