using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
//TODO Добавить описание
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
