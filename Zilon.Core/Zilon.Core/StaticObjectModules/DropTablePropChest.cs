using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    /// Контейнер со дропом, основанном на таблицах дропа.
    /// </summary>
    public class DropTablePropChest : ChestBase
    {
        [ExcludeFromCodeCoverage]
        public DropTablePropChest(
            IDropTableScheme[] dropTables,
            IDropResolver dropResolver) : base(new DropTableChestStore(dropTables, dropResolver))
        {
        }

        public override bool IsMapBlock => true;
    }
}