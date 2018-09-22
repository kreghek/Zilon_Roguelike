using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Аргументы событий, связанных с инвентарём.
    /// </summary>
    public class PropStoreEventArgs
    {
        public IProp[] Props { get; }

        [ExcludeFromCodeCoverage]
        public PropStoreEventArgs(IEnumerable<IProp> props)
        {
            Props = props.ToArray();
        }
    }
}
