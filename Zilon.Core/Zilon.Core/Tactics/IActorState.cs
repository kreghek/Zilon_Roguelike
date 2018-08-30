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
        float Hp { get; }

        /// <summary>
        /// Состояние актёра.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Инициатива актёра.
        /// </summary>
        float Initiative { get; }

        /// <summary>
        /// Происходит, если актёр умирает.
        /// </summary>
        event EventHandler Dead;

        void TakeDamage(float value);

        void SetHpForce(float hp);

        void RestoreHp(float value, float max);
    }
}
