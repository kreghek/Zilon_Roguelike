using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Хранилище, контент которого генерируется по таблице дропа.
    /// </summary>
    public sealed class DropTableChestStore : PropStoreBase
    {
        private readonly IEnumerable<DropTableScheme> _dropTables;
        private readonly IDropResolver _dropResolver;

        public DropTableChestStore(IEnumerable<DropTableScheme> dropTables, IDropResolver dropResolver)
        {
            _dropTables = dropTables;
            _dropResolver = dropResolver;
        }

        public override IProp[] CalcActualItems()
        {
            var props = GenerateProps();
            AddToContent(props);

            return base.CalcActualItems();
        }

        private void AddToContent(IProp[] props)
        {
            foreach (var prop in props)
            {
                _items.Add(prop);
            }
        }

        private IProp[] GenerateProps()
        {
            var props = _dropResolver.GetProps(_dropTables);
            return props;
        }
    }
}
