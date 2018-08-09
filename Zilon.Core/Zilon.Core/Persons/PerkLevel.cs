using System;

namespace Zilon.Core.Persons
{
    public sealed class PerkLevel
    {
        /// <summary>
        /// Основной уровень перка. Иначе - индекс схемы уровня.
        /// </summary>
        public int? Primary { get; }

        /// <summary>
        /// Подуровень. Или уровень внутри схемы уровня перка.
        /// </summary>
        public int Sub { get; }

        public PerkLevel(int? primary, int sub)
        {
            Primary = primary;
            Sub = sub;
        }

        [Obsolete("Если перк не получен, то его уровень должен быть null. Этого свойства быть не должно. Primary не Nullable.")]
        public static PerkLevel Zero
        {
            get
            {
                return new PerkLevel(null, 0);
            }
        }
    }
}
