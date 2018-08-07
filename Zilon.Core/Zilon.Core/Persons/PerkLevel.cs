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
    }
}
