using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Props;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Объект трансфера предметов.
    /// </summary>
    public class PropTransfer
    {
        public PropTransfer(
            [NotNull] IPropStore propStore,
            [NotNull] IEnumerable<IProp> added,
            [NotNull] IEnumerable<IProp> removed)
        {
            PropStore = propStore;
            Added = added.ToArray();
            Removed = removed.ToArray();
        }

        /// <summary>
        /// Добавляемые предметы в хранилище.
        /// </summary>
        public IProp[] Added { get; }

        /// <summary>
        /// Хранилище, с которым связан трансфер.
        /// </summary>
        public IPropStore PropStore { get; }

        /// <summary>
        /// Извлекаемые из хранилища предметы.
        /// </summary>
        public IProp[] Removed { get; }
    }
}