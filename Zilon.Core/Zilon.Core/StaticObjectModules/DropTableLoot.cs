using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    /// Реализация контейнера для выпавшего из монстра лута.
    /// </summary>
    public class DropTableLoot : ChestBase, ILootContainer
    {
        [ExcludeFromCodeCoverage]
        public DropTableLoot(
            IDropTableScheme[] dropTables,
            IDropResolver dropResolver) : base(new DropTableChestStore(dropTables, dropResolver))
        {
            Purpose = PropContainerPurpose.Loot;
        }

        public override bool IsMapBlock => false;
    }
}