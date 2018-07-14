using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейт предмета в секторе.
    /// </summary>
    public interface IPropContainer
    {
        /// <summary>
        /// Узер карты сектора, в котором находится контейнер.
        /// </summary>
        IMapNode Node { get; }

        /// <summary>
        /// Содержимое контейнера.
        /// </summary>
        IInventory Inventory { get; }
    }
}
