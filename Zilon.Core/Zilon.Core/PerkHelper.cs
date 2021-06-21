using System;
using System.Linq;

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
        public static int ConvertLevelSubsToTotal(IPerkScheme perkScheme, int level, int subLevel)
        {
            if (perkScheme is null)
            {
                throw new ArgumentNullException(nameof(perkScheme));
            }

            if (perkScheme.Levels?.Length < level)
            {
                throw new ArgumentException(
                    $"Specified level: {level} is less that levels in scheme: {perkScheme.Levels?.Length}.");
            }

            var levels = perkScheme.Levels;
            if (levels is null)
            {
                throw new ArgumentException("The scheme's levels is null.", nameof(perkScheme));
            }

            var sum = 0;
            for (var i = 1; i <= level; i++)
            {
                var perkLevelSubScheme = levels[i - 1];
                if (perkLevelSubScheme is null)
                {
                    throw new InvalidOperationException();
                }

                if (i < level)
                {
                    sum += perkLevelSubScheme.MaxValue;
                }
                else
                {
                    if (perkLevelSubScheme.MaxValue < subLevel)
                    {
                        throw new ArgumentException(
                            $"Specified sub: {subLevel} is less that sub in scheme: {perkLevelSubScheme.MaxValue}.");
                    }

                    sum += subLevel;
                }
            }

            return sum;
        }

        public static PerkLevel GetNextLevel(IPerkScheme perkScheme, PerkLevel level)
        {
            var currentTotal = ConvertLevelSubsToTotal(perkScheme, level.Primary, level.Sub);
            currentTotal++;

            if (!TryConvertTotalLevelToLevelSubs(perkScheme, currentTotal, out var nextLevel))
            {
                throw new InvalidOperationException("Perk has no next level.");
            }

            if (nextLevel is null)
            {
                // The inner method garantee nextLevel is not null if convertion is successful.
                // So it is unknown error.
                throw new InvalidOperationException("Unknown exception.");
            }

            return nextLevel;
        }

        public static bool HasNextLevel(IPerkScheme perkScheme, PerkLevel level)
        {
            return GetNextLevelToCheckNext(perkScheme, level);
        }

        /// <summary>
        /// Преобразование суммарного уровня в уровень/подуровень для конкретной схемы перка.
        /// </summary>
        /// <param name="perkScheme">Схема.</param>
        /// <param name="totalLevel">Суммарный уровень.</param>
        public static bool TryConvertTotalLevelToLevelSubs(IPerkScheme perkScheme, int totalLevel,
            out PerkLevel? perkLevel)
        {
            if (perkScheme is null)
            {
                throw new ArgumentNullException(nameof(perkScheme));
            }

            if (totalLevel == 0)
            {
                throw new ArgumentException("Total must be more that zero.", nameof(totalLevel));
            }

            if (perkScheme.Levels is null)
            {
                throw new ArgumentException("Scheme's levels must not be null.", nameof(perkScheme));
            }

            if (perkScheme.Levels.Length == 0)
            {
                throw new ArgumentException("Scheme's levels must notbe empty.", nameof(perkScheme));
            }

            foreach (var schemeLevel in perkScheme.Levels)
            {
                if (schemeLevel?.MaxValue <= 0)
                {
                    throw new ArgumentException("Scheme must contains no zeros.", nameof(perkScheme));
                }
            }

            var schemeLevels = perkScheme.Levels.Select(x => x!.MaxValue).ToArray();

            try
            {
                var isSuccess = TryConvertTotalIntoLevelSubsInner(schemeLevels, totalLevel, out var levelInner,
                    out var subInner);

                perkLevel = null;
                if (isSuccess)
                {
                    perkLevel = new PerkLevel(levelInner, subInner);
                }

                return isSuccess;
            }
            catch (ArgumentException exception)
            {
                if (exception.ParamName == "total")
                {
                    throw new ArgumentException($"Total {totalLevel} is too big", nameof(totalLevel), exception);
                }

                throw;
            }
        }

        private static bool GetNextLevelToCheckNext(IPerkScheme perkScheme, PerkLevel level)
        {
            var currentTotal = ConvertLevelSubsToTotal(perkScheme, level.Primary, level.Sub);
            currentTotal++;

            return TryConvertTotalLevelToLevelSubs(perkScheme, currentTotal, out var _);
        }

        private static bool TryConvertTotalIntoLevelSubsInner(int[] scheme, int total, out int lvl, out int sub)
        {
            var levelMax = 1;
            var currentTotal = total;

            for (var i = 0; i < scheme.Length; i++)
            {
                if (scheme[i] >= currentTotal)
                {
                    lvl = levelMax;
                    sub = currentTotal;
                    return true;
                }

                currentTotal -= scheme[i];
                levelMax++;
            }

            // This means `total` was be more that sum of levels.

            lvl = 0;
            sub = 0;
            return false;
        }
    }
}