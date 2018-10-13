using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="propScheme"> Схема экипировки. </param>
        /// <param name="acts"> Действия, которые может дать эта экипировка. </param>
        /// <exception cref="T:System.ArgumentException">
        /// Выбрасывает, если на вход подана схема,
        /// не содержащая характеристики экипировки <see cref="P:Zilon.Core.Schemes.PropScheme.Equip" />.
        /// </exception>
        public Equipment(IPropScheme propScheme, IEnumerable<ITacticalActScheme> acts) : base(propScheme)
        {
            if (propScheme.Equip == null)
            {
                throw new ArgumentException("Не корректная схема.", nameof(propScheme));
            }

            Power = 1;

            if (acts != null)
            {
                Acts = acts.ToArray();
            }
            else
            {
                Acts = new TacticalActScheme[0];
            }
        }

        public ITacticalActScheme[] Acts { get; }

        /// <summary>
        /// Мощь/качество/уровень экипировки.
        /// </summary>
        public int Power { get; set; }
    }
}
