using System;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс состояния актёра.
    /// </summary>
    public interface IActorState
    {
        /// <summary>
        /// Текущий запас хитпоинтов.
        /// </summary>
        int Hp { get; }

        /// <summary>
        /// Текущий запас очков действия.
        /// </summary>
        int Ap { get; }

        /// <summary>
        /// Текущий запас очков манёвра.
        /// </summary>
        int Mp { get; }

        /// <summary>
        /// Состояние актёра.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Происходит, если актёр умирает.
        /// </summary>
        event EventHandler Dead;

        /// <summary>
        /// Получение урона.
        /// </summary>
        /// <param name="value"> Величина урона с учётом всех модификаторов. </param>
        void TakeDamage(int value);

        /// <summary>
        /// Форсированно установить запас здоровья.
        /// </summary>
        /// <param name="hp"> Целевое значение запаса здоровья. </param>
        void SetHpForce(int hp);

        /// <summary>
        /// Восстановить очки здоровья.
        /// </summary>
        /// <param name="value"> Количество восстановленных очков здоровья. </param>
        /// <param name="max"> Максимальное количество очков здоровья. </param>
        void RestoreHp(float value, float max);
    }
}
