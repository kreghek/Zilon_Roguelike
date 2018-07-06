using System;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <inheritdoc />
    /// <summary>
    /// Экипировка персонажа.
    /// </summary>
    public class Equipment : PropBase
    {
        /// <inheritdoc />
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="scheme"> Схема экипировки. </param>
        /// <param name="acts"> Действия, которые может дать эта экипировка. </param>
        /// <exception cref="T:System.ArgumentException">
        /// Выбрасывает, если на вход подана схема,
        /// не содержащая характеристики экипировки <see cref="P:Zilon.Core.Schemes.PropScheme.Equip" />.
        /// </exception>
        public Equipment(PropScheme scheme, ITacticalAct[] acts) : base(scheme)
        {
            if (scheme.Equip == null)
            {
                throw new ArgumentException("Не корректная схема.", nameof(scheme));
            }

            Acts = acts;
        }

        public ITacticalAct[] Acts { get; }
    }
}
