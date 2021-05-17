using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zilon.Core.Props
{
    /// <summary>
    /// Аргументы событий, связанных с инвентарём.
    /// </summary>
    public class PropStoreEventArgs
    {
        [ExcludeFromCodeCoverage]
        public PropStoreEventArgs(IEnumerable<IProp> props)
        {
            if (props == null)
            {
                throw new ArgumentNullException(nameof(props));
            }

            Props = props.ToArray();
        }

        [ExcludeFromCodeCoverage]
        public PropStoreEventArgs(params IProp[] props)
        {
            Props = props ?? throw new ArgumentNullException(nameof(props));
        }

        public IProp[] Props { get; }
    }
}