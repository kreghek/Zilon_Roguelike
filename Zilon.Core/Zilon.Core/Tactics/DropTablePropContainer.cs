using System;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Контейнер со дропом, основанном на таблицах дропа.
    /// </summary>
    public class DropTablePropContainer : IPropContainer
    {
        private bool _isOpened;

        public IMapNode Node { get; }

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

        public int Id { get; set; }

        public DropTablePropContainer(IMapNode node,
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
