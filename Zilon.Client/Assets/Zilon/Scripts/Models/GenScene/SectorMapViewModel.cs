using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

public class SectorMapViewModel : MonoBehaviour
{
    public MapNodeVM MapNodePrefab;

    public IEnumerable<MapNodeVM> NodeViewModels { get; private set; }

    public void Init(ISectorMap map)
    {
        var nodeViewModels = InitNodeViewModels(map);
        NodeViewModels = nodeViewModels;
    }

    private List<MapNodeVM> InitNodeViewModels(ISectorMap map)
    {
        var nodeVMs = new List<MapNodeVM>();

        foreach (var node in map.Nodes)
        {
            var mapNodeVm = Instantiate(MapNodePrefab, transform);

            var hexNode = (HexNode)node;
            var nodeWorldPositionParts = HexHelper.ConvertToWorld(hexNode.OffsetX, hexNode.OffsetY);
            var worldPosition = new Vector3(nodeWorldPositionParts[0], nodeWorldPositionParts[1] / 2);
            mapNodeVm.transform.position = worldPosition;
            mapNodeVm.Node = hexNode;
            mapNodeVm.Neighbors = map.GetNext(node).Cast<HexNode>().ToArray();

            if (map.Transitions.ContainsKey(node))
            {
                mapNodeVm.IsExit = true;
            }

            nodeVMs.Add(mapNodeVm);
        }

        return nodeVMs;
    }
}

