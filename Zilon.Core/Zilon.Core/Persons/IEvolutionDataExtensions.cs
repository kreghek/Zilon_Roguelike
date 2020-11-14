using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.PersonModules;

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
        /// <param name="evolutionModule"> Модуль эволюции. </param>
        /// <returns> Коллекция прокаченных перков или пустая коллекция, если перки модуля эволюции равны null. </returns>
        /// <remarks>
        /// Полученными перками считаются перки, которые прокачены хотя бы на один уровень или встроенные перки
        /// (те, которые нельзя прокачать, только получить при рождении или за заслуги).
        /// </remarks>
        public static IEnumerable<IPerk> GetArchievedPerks([NotNull] this IEvolutionModule evolutionModule)
        {
            if (evolutionModule == null)
            {
                throw new ArgumentNullException(nameof(evolutionModule));
            }

            var archievedPerks = evolutionModule.Perks?.Where(x => (x.CurrentLevel != null) || x.Scheme.IsBuildIn);
            if (archievedPerks == null)
            {
                archievedPerks = Array.Empty<IPerk>();
            }

            return archievedPerks;
        }
    }
}