using System;
using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    public sealed class MonsterSectorFowData : ISectorFowData
    {
        public IEnumerable<SectorMapFowNode> Nodes => Array.Empty<SectorMapFowNode>();

        public void AddNodes(IEnumerable<SectorMapFowNode> nodes)
        {
            // Ничего не делаем. Просто метод для соблюдения интерфейса.
        }
    }
}
