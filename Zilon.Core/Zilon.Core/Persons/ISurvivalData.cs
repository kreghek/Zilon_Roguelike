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
        /// Снижение характеристики.
        /// </summary>
        /// <param name="type"> Тип характеритсики, которая будет произведено влияние. </param>
        /// <param name="value"> Значение, на которое снижается текущий запас. </param>
        void DecreaseStat(SurvivalStatType type, int value);

        /// <summary>
        /// Пополнение запаса характеристики.
        /// </summary>
        /// <param name="type"> Тип характеритсики, которая будет произведено влияние. </param>
        /// <param name="value"> Значение, на которое восстанавливается текущий запас. </param>
        void RestoreStat(SurvivalStatType type, int value);

        /// <summary>
        /// Форсированно установить запас здоровья.
        /// </summary>
        /// <param name="type"> Тип характеритсики, которая будет произведено влияние. </param>
        /// <param name="value"> Целевое значение запаса характеристики. </param>
        void SetStatForce(SurvivalStatType type, int value);

        /// <summary>
        /// Признак того, что персонаж мёртв.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Происходит, если персонаж умирает.
        /// </summary>
        event EventHandler Dead;

        /// <summary>
        /// Событие, которое происходит, если значение характеристики
        /// пересекает ключевое значение (мин/макс/четверти/0).
        /// </summary>
        event EventHandler<SurvivalStatChangedEventArgs> StatCrossKeyValue;
    }
}
