using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс предмета.
    /// </summary>
    public interface IProp
    {
        /// <summary>
        /// Схема предмета.
        /// </summary>
        IPropScheme Scheme { get; }
    }
}