using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Информация о перке.
    /// </summary>
    public interface IPerk: IJobExecutable
    {
        /// <summary>
        /// Схема перка.
        /// </summary>
        PerkScheme Scheme { get; }
    }
}
