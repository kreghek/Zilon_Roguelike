using System;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// реализация контейнера для выпавшего из монстра лута.
    /// </summary>
    public class DropTableLoot: IPropContainer
    {
        public IMapNode Node { get; }

        public IPropStore Content { get; }
        public bool IsOpened
        {
            get => true; set
            {
                // Пустая реализация, потому что 
                // контейнеры лута всегда открыты.
            }
        }

        public int Id { get; set; }

        public bool IsMapBlock => false;

        public DropTableLoot(IMapNode node,
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
