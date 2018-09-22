using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Контейнер со дропом, основанном на таблицах дропа.
    /// </summary>
    public class DropTablePropChest : IPropContainer
    {
        private bool _isOpened;

        [ExcludeFromCodeCoverage]
        public IMapNode Node { get; }

        [ExcludeFromCodeCoverage]
        public IPropStore Content { get; }

        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                _isOpened = value;
                DoSetIsOpened();
            }
        }

        [ExcludeFromCodeCoverage]
        public int Id { get; set; }

        [ExcludeFromCodeCoverage]
        public bool IsMapBlock => true;

        public DropTablePropChest(IMapNode node,
            DropTableScheme[] dropTables,
            IDropResolver dropResolver)
        {
            Node = node;
            Content = new DropTableChestStore(dropTables, dropResolver);
        }

        public event EventHandler IsOpenChanged;

        private void DoSetIsOpened()
        {
            IsOpenChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
