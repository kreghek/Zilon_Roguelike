using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Данные персонажа по выживанию.
    /// </summary>
    /// <remarks>
    /// Здесь будут сведения о питании, отдыхе, ранах, эмоциональном состоянии персонажа.
    /// </remarks>
    public interface ISurvivalData
    {
        /// <summary>
        /// Текущие характеристики.
        /// </summary>
        SurvivalStat[] Stats { get; }

        /// <summary>
        /// Обновление состояния данных о выживании.
        /// </summary>
        void Update();

        /// <summary>
        /// Пополнение запаса характеристики.
        /// </summary>
        /// <param name="type"> Тип характеритсики, которая будет восстанавливаться. </param>
        /// <param name="value"> Значение, на которое увеличивается текущий запас. </param>
        void RestoreStat(SurvivalStatTypes type, int value);

        /// <summary>
        /// Событие, которое происходит, если значение характеристики пересекает ключевое значение (мин/макс/четверти/0).
        /// </summary>
        event EventHandler<SurvivalStatChangedEventArgs> StatCrossKeyValue;
    }
}
