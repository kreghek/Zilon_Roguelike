using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Эффект персонажа.
    /// </summary>
    public interface IPersonEffect
    {
        /// <summary>
        /// Получить правила эффекта.
        /// </summary>
        /// <returns> Возвращает текущие правила эффекта. </returns>
        EffectRule[] GetRules();

        /// <summary>
        /// Выстреливает, когда внешние характеристики эффекта изменились.
        /// </summary>
        event EventHandler Changed;
    }
}