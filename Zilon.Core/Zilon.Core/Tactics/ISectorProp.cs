using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейт предмета в секторе.
    /// </summary>
    public interface ISectorProp
    {
        /// <summary>
        /// Фактический предмет.
        /// </summary>
        IProp Prop { get; }
    }
}
