using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Информация о перке.
    /// </summary>
    public interface IPerk : IJobExecutable
    {
        /// <summary>
        /// Схема перка.
        /// </summary>
        IPerkScheme Scheme { get; }

        /// <summary>
        /// Текущий уровень перка. Иначе - индекс схемы уровня.
        /// </summary>
        PerkLevel CurrentLevel { get; set; }
    }
}