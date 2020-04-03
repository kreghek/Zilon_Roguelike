using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    public interface ISectorFowData
    {
        IEnumerable<SectorMapFowNode> Nodes { get; }

        void AddNodes(IEnumerable<SectorMapFowNode> nodes);

        SectorMapFowNode GetNode(IGraphNode node);

        void ChangeNodeState(SectorMapFowNode node, SectorMapNodeFowState targetState);

        IEnumerable<SectorMapFowNode> GetFowNodeByState(SectorMapNodeFowState targetState);
    }
}
