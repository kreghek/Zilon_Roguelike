using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Контейнер со дропом, основанном на таблицах дропа.
    /// </summary>
    public class DropTablePropChest : ChestBase
    {
        public override bool IsMapBlock => true;

        [ExcludeFromCodeCoverage]
        public DropTablePropChest(IMapNode node,
            DropTableScheme[] dropTables,
            IDropResolver dropResolver):base(node, new DropTableChestStore(dropTables, dropResolver))
        {

        }
    }
}
