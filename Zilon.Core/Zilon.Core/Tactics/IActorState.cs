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
        void RestoreHp(int value, int max);
    }
}
