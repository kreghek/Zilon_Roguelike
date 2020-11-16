using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    public interface ISectorFowData
    {
        IEnumerable<SectorMapFowNode> Nodes { get; }

        void AddNodes(IEnumerable<SectorMapFowNode> nodes);

        void ChangeNodeState(SectorMapFowNode node, SectorMapNodeFowState targetState);

        IEnumerable<SectorMapFowNode> GetFowNodeByState(SectorMapNodeFowState targetState);

        SectorMapFowNode GetNode(IGraphNode node);
    }
}