using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация контейнера для выпавшего из монстра лута.
    /// </summary>
    public class DropTableLoot : IPropContainer
    {
        [ExcludeFromCodeCoverage]
        public IMapNode Node { get; }

        [ExcludeFromCodeCoverage]
        public IPropStore Content { get; }

        [ExcludeFromCodeCoverage]
        public bool IsOpened
        {
            get => true;
            set
            {
                // Пустая реализация, потому что 
                // контейнеры лута всегда открыты.
            }
        }

        [ExcludeFromCodeCoverage]
        public int Id { get; set; }

        [ExcludeFromCodeCoverage]
        public bool IsMapBlock => false;

        [ExcludeFromCodeCoverage]
        public DropTableLoot(IMapNode node,
            DropTableScheme[] dropTables,
            IDropResolver dropResolver)
        {
            Node = node;
            Content = new DropTableChestStore(dropTables, dropResolver);
        }

        public event EventHandler IsOpenChanged;
    }
}
