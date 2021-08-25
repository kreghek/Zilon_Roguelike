﻿using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Props
{
    /// <inheritdoc />
    /// <summary>
    /// Экипировка персонажа.
    /// </summary>
    public class Equipment : PropBase
    {
        private const int EQUIPMENT_DURABLE = 300;

        private readonly string _name;

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
        public Equipment(IPropScheme propScheme,
            IEnumerable<ITacticalActScheme> acts) :
            base(propScheme)
        {
            if (propScheme.Equip is null)
            {
                throw new ArgumentException(
                    "The scheme is not valid for equipment. It must has not null Equip subscheme.", nameof(propScheme));
            }

            _name = string.Empty;

            if (acts != null)
            {
                Acts = acts.ToArray();
            }
            else
            {
                Acts = Array.Empty<ITacticalActScheme>();
            }

            Durable = new Stat(EQUIPMENT_DURABLE, 0, EQUIPMENT_DURABLE);
        }

        public Equipment(IPropScheme propScheme) : this(propScheme, Array.Empty<ITacticalActScheme>())
        {
        }

        public Equipment(IPropScheme propScheme,
            IEnumerable<ITacticalActScheme> acts,
            string name) :
            this(propScheme, acts)
        {
            _name = name;
        }

        public IEnumerable<ITacticalActScheme> Acts { get; }

        /// <summary>Прочность предмета.</summary>
        public Stat Durable { get; }

        public override string ToString()
        {
            if (_name != null)
            {
                return $"{_name} {base.ToString()}";
            }

            return base.ToString();
        }
    }
}