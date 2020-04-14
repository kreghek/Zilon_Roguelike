using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Graphs;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Контейнер со дропом, основанном на таблицах дропа.
    /// </summary>
    public class DropTablePropChest : ChestBase
    {
        public override bool IsMapBlock => true;

        [ExcludeFromCodeCoverage]
        public DropTablePropChest(IGraphNode node,
            IDropTableScheme[] dropTables,
            IDropResolver dropResolver) : base(new DropTableChestStore(dropTables, dropResolver))
        {
        }

        [ExcludeFromCodeCoverage]
        public DropTablePropChest(IGraphNode node,
            IDropTableScheme[] dropTables,
            IDropResolver dropResolver,
            int id) : base(new DropTableChestStore(dropTables, dropResolver))
        {
        }
    }
}
