using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация контейнера для выпавшего из монстра лута.
    /// </summary>
    public class DropTableLoot : ChestBase, ILootContainer
    {
        public override bool IsMapBlock => false;

        [ExcludeFromCodeCoverage]
        public DropTableLoot(IMapNode node,
            IDropTableScheme[] dropTables,
            IDropResolver dropResolver) : base(node, new DropTableChestStore(dropTables, dropResolver))
        {

        }
    }
}
