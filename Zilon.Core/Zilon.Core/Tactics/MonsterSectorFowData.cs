using System;
using System.Collections.Generic;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    public sealed class MonsterSectorFowData : ISectorFowData
    {
        public IEnumerable<SectorMapFowNode> Nodes => Array.Empty<SectorMapFowNode>();

        public void AddNodes(IEnumerable<SectorMapFowNode> nodes)
        {
            // Ничего не делаем. Просто метод для соблюдения интерфейса.
        }

        public SectorMapFowNode GetNode(IGraphNode node)
        {
            // Возвращаем null, потому что этот объект не предполагает хранение чего-либо.
            return null;
        }
    }
}
