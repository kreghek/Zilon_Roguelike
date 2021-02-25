using System;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core
{
    public static class PerkHelper
    {
        /// <summary>
        /// Преобразование уровня/подуровня в суммарный уровень.
        /// </summary>
        /// <param name="perkScheme">Схема.</param>
        /// <param name="level">Уровень перка.</param>
        /// <param name="subLevel">Подуровень перка.</param>
        /// <returns></returns>
        public static int? ConvertLevel(IPerkScheme perkScheme, int? level, int subLevel)
        {
            if (perkScheme is null)
            {
                throw new ArgumentNullException(nameof(perkScheme));
            }

            if (level == null)
            {
                return null;
            }

            var sum = 0;
            for (var i = 1; i <= level; i++)
            {
                if (i < level)
                {
                    sum += perkScheme.Levels[i - 1].MaxValue;
                }
                else
                {
                    sum += subLevel;
                }
            }

            return sum;
        }

        /// <summary>
        /// Преобразование суммарного уровня в уровень/подуровень для конкретной схемы перка.
        /// </summary>
        /// <param name="perkScheme">Схема.</param>
        /// <param name="totalLevel">Суммарный уровень.</param>
        /// <param name="level">Уровень перка.</param>
        /// <param name="subLevel">Подуровень перка.</param>
        public static PerkLevel ConvertTotalLevel(IPerkScheme perkScheme, int totalLevel)
        {
            if (perkScheme is null)
            {
                throw new ArgumentNullException(nameof(perkScheme));
            }

            if (totalLevel == 0)
            {
                throw new ArgumentException("Total must be more that zero.", nameof(totalLevel));
            }

            foreach (var schemeLevel in perkScheme.Levels)
            {
                if (schemeLevel.MaxValue <= 0)
                {
                    throw new ArgumentException("Scheme must contains no zeros.", nameof(perkScheme));
                }
            }

            var schemeLevels = perkScheme.Levels.Select(x => x.MaxValue).ToArray();

            ConvertTotalIntoLevelSubsInner(schemeLevels, totalLevel, out var levelInner, out var subInner);

            var perkLevel = new PerkLevel(levelInner, subInner);

            return perkLevel;
        }

        [NotNull]
        public static PerkLevel GetNextLevel([NotNull] IPerkScheme perkScheme, [NotNull] PerkLevel level)
        {
            var currentTotal = ConvertLevel(perkScheme, level.Primary, level.Sub);
            currentTotal++;

            var nextLevel = ConvertTotalLevel(perkScheme, currentTotal.Value);
            return nextLevel;
        }

        public static bool HasNextLevel(IPerkScheme perkScheme, PerkLevel level)
        {
            try
            {
                GetNextLevel(perkScheme, level);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ConvertTotalIntoLevelSubsInner(int[] scheme, int total, out int lvl, out int sub)
        {
            var levelMax = 1;
            var currentTotal = total;

            for (var i = 0; i < scheme.Length; i++)
            {
                if (scheme[i] >= currentTotal)
                {
                    lvl = levelMax;
                    sub = currentTotal;
                    return;
                }

                currentTotal -= scheme[i];
                levelMax++;
            }

            // This means `total` was be more that sum of levels.

            throw new ArgumentException($"Total {total} is too big for that schemes: ${string.Join(", ", scheme)}.", nameof(total));
        }
    }
}