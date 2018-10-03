namespace Zilon.Core.Components
{
    public enum DefenceType
    {
        /// <summary>
        /// Неопределённое.
        /// </summary>
        /// <remarks>
        /// Если выбрано, то, скорее всего, ошибка.
        /// </remarks>
        Undefined,

        TacticalDefence,
        FuryDefence,
        ShadowDefence,
        TrickyDefence,
        ConcentratedDefence,
        RapidDefence,

        /// <summary>
        /// Даёт бонус против всех типов наступления. Кроме божественного.
        /// </summary>
        DivineDefence
    }
}
