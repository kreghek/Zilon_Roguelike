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
            {
                return null;
            }

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
        public static void ConvertTotalLevel(IPerkScheme perkScheme, int totalLevel, out int? level, out int? subLevel)
        {
            if (perkScheme is null)
            {
                throw new ArgumentNullException(nameof(perkScheme));
            }
            
            var schemeLevels = perkScheme.Levels.Select(x=>x.MaxValue).ToArray();
            
            int levelInner;
            int subInner;
            
            ConvertTotalIntoLevelSubsInner(schemeLevels, totalLevel, out levelInner, out subInner);
            
            level = levelInner;
            subLevel = subInner;
        }
        
        private static ConvertTotalIntoLevelSubsInner(int[] scheme, int total, out int lvl, out int sub)
        {
            throw new NotImplemented();
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
}
