using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Характеристики, используемые персонажем в бою.
    /// </summary>
    public interface ICombatStats
    {
        /// <summary>
        /// Навык рукопашного боя.
        /// </summary>
        PersonStat Melee { get; }

        /// <summary>
        /// Навык стрельбы.
        /// </summary>
        PersonStat Ballistic { get; }

        /// <summary>
        /// Знание техники.
        /// </summary>
        /// <remarks>
        /// Влияет применение действий, требующих использование сложных
        /// технических устройств.
        /// </remarks>
        PersonStat Tech { get; }

        /// <summary>
        /// Медицина.
        /// </summary>
        PersonStat Medic { get; }

        /// <summary>
        /// Псионические способности.
        /// </summary>
        /// <remarks>
        /// Магия, гипноз и т.д.
        /// </remarks>
        PersonStat Psy { get; }

        /// <summary>
        /// Понимание социума.
        /// </summary>
        /// <remarks>
        /// Влияет на действия, требующих влияние на индивидуумов общества.
        /// Например, страх или насмешка.
        /// </remarks>
        PersonStat Social { get; }
    }
}
