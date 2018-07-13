using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Аргументы событий, связанных с инвентарём.
    /// </summary>
    public class InventoryEventArgs
    {
        public IProp[] Props { get; }

        public InventoryEventArgs(IEnumerable<IProp> props)
        {
            Props = props.ToArray();
        }
    }
}
