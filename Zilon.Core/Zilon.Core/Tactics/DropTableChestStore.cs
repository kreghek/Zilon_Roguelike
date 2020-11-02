using System.Collections.Generic;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Хранилище, контент которого генерируется по таблице дропа.
    /// </summary>
    public sealed class DropTableChestStore : PropStoreBase
    {
        private readonly IEnumerable<IDropTableScheme> _dropTables;
        private readonly IDropResolver _dropResolver;

        /// <summary>
        /// Внутренний ключ, указывающий, что разрешение контента уже произведено.
        /// </summary>
        private bool _contentResolved;

        public DropTableChestStore(IEnumerable<IDropTableScheme> dropTables, IDropResolver dropResolver)
        {
            _dropTables = dropTables;
            _dropResolver = dropResolver;
        }

        public override IProp[] CalcActualItems()
        {
            if (_contentResolved)
            {
                return base.CalcActualItems();
            }

            var props = GenerateProps();
            AddToContent(props);
            _contentResolved = true;

            return base.CalcActualItems();
        }

        private void AddToContent(IProp[] props)
        {
            foreach (var prop in props)
            {
                Add(prop);
            }
        }

        private IProp[] GenerateProps()
        {
            var props = _dropResolver.Resolve(_dropTables);
            return props;
        }
    }
}