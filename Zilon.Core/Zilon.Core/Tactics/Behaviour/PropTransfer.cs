using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Props;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Объект трансфера предметов.
    /// </summary>
    public class PropTransfer
    {
        public PropTransfer(IPropStore propStore, IEnumerable<IProp> added, IEnumerable<IProp> removed)
        {
            PropStore = propStore;
            Added = added.ToArray();
            Removed = removed.ToArray();
        }

        /// <summary>
        /// Хранилище, с которым связан трансфер.
        /// </summary>
        public IPropStore PropStore { get; }

        /// <summary>
        /// Добавляемые предметы в хранилище.
        /// </summary>
        public IProp[] Added { get; }

        /// <summary>
        /// Извлекаемые из хранилища предметы.
        /// </summary>
        public IProp[] Removed { get; }
        
    }
}
