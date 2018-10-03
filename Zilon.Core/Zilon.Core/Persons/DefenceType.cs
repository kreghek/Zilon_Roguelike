namespace Zilon.Core.Persons
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

        Tactical,
        Fury,
        Shadow,
        Tricky,
        Force,
        Rapid,

        /// <summary>
        /// Даёт бонус против всех типов наступления. Кроме божественного.
        /// </summary>
        Divine
    }
}
