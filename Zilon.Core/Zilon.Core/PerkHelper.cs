using System;

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
                return null;

            var sum = 0;
            for (var i = 0; i <= level; i++)
            {
                if (i < level)
                {
                    sum += perkScheme.Levels[i].MaxValue + 1;
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
        public static PerkSchemeLevel ConvertTotalLevel(
            IPerkScheme perkScheme,
            int totalLevel)
        {
            if (perkScheme is null)
            {
                throw new ArgumentNullException(nameof(perkScheme));
            }

            var levelRemains = totalLevel;
            var currentLevelPointer = 0;
            int? level;
            int? subLevel;
            do
            {
                if (currentLevelPointer >= perkScheme.Levels.Length)
                {
                    return new PerkSchemeLevel(level: null, subLevel: null);
                }

                var levelScheme = perkScheme.Levels[currentLevelPointer];
                level = currentLevelPointer + 1;
                subLevel = Math.Min(levelRemains, levelScheme.MaxValue);
                currentLevelPointer++;

                var currentLevelCapability = levelScheme.MaxValue;
                levelRemains -= currentLevelCapability;
            } while (levelRemains >= 0);

            return new PerkSchemeLevel(level, subLevel);
        }

        public static PerkLevel GetNextLevel(IPerkScheme perkScheme, PerkLevel level)
        {
            if (perkScheme is null)
            {
                throw new ArgumentNullException(nameof(perkScheme));
            }

            if (level == null)
            {
                return new PerkLevel(0, 0);
            }

            var currentLevel = level.Primary;
            var currentSubLevel = level.Sub;

            var currentLevelScheme = perkScheme.Levels[currentLevel];

            if (currentSubLevel + 1 > currentLevelScheme.MaxValue)
            {
                currentSubLevel = 0;
                currentLevel++;
            }
            else
            {
                currentSubLevel++;
            }

            return new PerkLevel(currentLevel, currentSubLevel);
        }
    }

    /// <summary>
    /// Scheme level and sublevel in one structure.
    /// </summary>
    public class PerkSchemeLevel
    {
        public PerkSchemeLevel(int? level, int? subLevel)
        {
            Level = level;
            SubLevel = subLevel;
        }

        public int? Level { get; }

        public int? SubLevel { get; }
    }
}