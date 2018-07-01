using System;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Экипировка персонажа.
    /// </summary>
    public class Equipment : PropBase
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="scheme"> Схема экипировки. </param>
        /// <exception cref="ArgumentException">
        /// Выбрасывает, если на вход подана схема,
        /// не содержащая характеристики экипировки <see cref="PropScheme.Equip"/>.
        /// </exception>
        public Equipment(PropScheme scheme) : base(scheme)
        {
            if (scheme.Equip == null)
            {
                throw new ArgumentException("Не корректная схема.", nameof(scheme));
            }
        }
    }
}
