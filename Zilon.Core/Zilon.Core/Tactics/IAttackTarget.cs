using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс объект, который возможно атаковать.
    /// </summary>
    public interface IAttackTarget
    {
        /// <summary>
        /// Текущий узел карты, в котором находится цель.
        /// </summary>
        HexNode Node { get; }

        /// <summary>
        /// Принятие урона.
        /// </summary>
        /// <param name="value">Значение, которое нужно принять.</param>
        void TakeDamage(float value);

        /// <summary>
        /// Проверка, может ли цель быть атакована.
        /// </summary>
        bool CanBeDamaged();
    }
}
