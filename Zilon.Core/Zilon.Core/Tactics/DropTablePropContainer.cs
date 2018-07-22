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
        public IMapNode Node { get; private set; }

        public IPropStore Content { get; }

        public DropTablePropContainer(IMapNode node,
            DropTableScheme[] dropTables,
            IDropResolver dropResolver)
        {
            Node = node;
            Content = new DropTableChestStore(dropTables, dropResolver);
        }
    }
}
