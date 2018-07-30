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
        float Melee { get; }

        /// <summary>
        /// Навык стрельбы.
        /// </summary>
        float Ballistic { get; }

        /// <summary>
        /// Знание техники.
        /// </summary>
        /// <remarks>
        /// Влияет применение действий, требующих использование сложных
        /// технических устройств.
        /// </remarks>
        float Tech { get; }

        /// <summary>
        /// Медицина.
        /// </summary>
        float Medic { get; }

        /// <summary>
        /// Псионические способности.
        /// </summary>
        /// <remarks>
        /// Магия, гипноз и т.д.
        /// </remarks>
        float Psy { get; }

        /// <summary>
        /// Понимание социума.
        /// </summary>
        /// <remarks>
        /// Влияет на действия, требующих влияние на индивидуумов общества.
        /// Например, страх или насмешка.
        /// </remarks>
        float Social { get; }
    }
}
