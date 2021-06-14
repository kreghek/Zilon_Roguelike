using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Эффект персонажа.
    /// </summary>
    public interface IPersonCondition
    {
        /// <summary>
        /// Получить правила эффекта.
        /// </summary>
        /// <returns> Возвращает текущие правила эффекта. </returns>
        ConditionRule[] GetRules();

        /// <summary>
        /// Выстреливает, когда внешние характеристики эффекта изменились.
        /// </summary>
        event EventHandler? Changed;
    }
}