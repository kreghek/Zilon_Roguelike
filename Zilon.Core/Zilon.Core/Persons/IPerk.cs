using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Информация о перке.
    /// </summary>
    public interface IPerk : IJobExecutable
    {
        /// <summary>
        /// Текущий уровень перка. Иначе - индекс схемы уровня.
        /// </summary>
        PerkLevel CurrentLevel { get; set; }

        /// <summary>
        /// Схема перка.
        /// </summary>
        IPerkScheme Scheme { get; }
    }
}