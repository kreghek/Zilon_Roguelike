using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Расширения модуля развития персонажа.
    /// </summary>
    public static class IEvolutionDataExtensions
    {
        /// <summary>
        /// Возвращает все развитиые перки персонажа.
        /// </summary>
        /// <param name="evolutionData"> Модуль эволюции. </param>
        /// <returns> Коллекция прокаченных перков или пустая коллекция, если перки модуля эволюции равны null. </returns>
        /// <remarks>
        /// Полученными перками считаются перки, которые прокачены хотя бы на один уровень.
        /// </remarks>
        public static IEnumerable<IPerk> GetArchievedPerks([NotNull] this IEvolutionData evolutionData)
        {
            if (evolutionData == null)
            {
                throw new ArgumentNullException(nameof(evolutionData));
            }

            var archievedPerks = evolutionData.Perks?.Where(x => x.CurrentLevel != null);
            if (archievedPerks == null)
            {
                archievedPerks = new IPerk[0];
            }

            return archievedPerks;
        }
    }
}
