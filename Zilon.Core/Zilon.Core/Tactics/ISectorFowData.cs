using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    public interface ISectorFowData
    {
        IEnumerable<SectorMapFowNode> Nodes { get; }

        void AddNodes(IEnumerable<SectorMapFowNode> nodes);
    }
}
