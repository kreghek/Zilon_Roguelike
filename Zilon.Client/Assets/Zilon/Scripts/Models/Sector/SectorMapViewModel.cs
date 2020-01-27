using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Models.Sector;

using UnityEngine;

using Zenject;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

public class SectorMapViewModel : MonoBehaviour
{
    [Inject]
    private readonly DiContainer _diContainer;

    public MapNodeVM MapNodePrefab;

    public IEnumerable<MapNodeVM> NodeViewModels { get; private set; }

    public event EventHandler<NodeInteractEventArgs> NodeSelected;

    public event EventHandler<NodeInteractEventArgs> NodeEnter;

    public void Init(ISectorMap map)
    {
        var nodeViewModels = InitNodeViewModels(map);
        NodeViewModels = nodeViewModels;
    }

    private List<MapNodeVM> InitNodeViewModels(ISectorMap map)
    {
        var nodeViewModels = new List<MapNodeVM>();

        foreach (var node in map.Nodes)
        {
            var mapNodeObj = _diContainer.InstantiatePrefab(MapNodePrefab, transform);

            var mapNodeVm = mapNodeObj.GetComponent<MapNodeVM>();

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

            nodeViewModels.Add(mapNodeVm);

            mapNodeVm.OnSelect += MapNodeVm_Select;
            mapNodeVm.MouseEnter += MapNodeVm_MouseEnter;

            var fowController = mapNodeObj.GetComponent<FowNodeController>();
            fowController.SectorMap = map;
        }

        return nodeViewModels;
    }

    private void MapNodeVm_MouseEnter(object sender, EventArgs e)
    {
        var nodeViewModel = (MapNodeVM)sender;
        var eventArgs = new NodeInteractEventArgs(nodeViewModel);
        NodeEnter?.Invoke(this, eventArgs);

    }

    private void MapNodeVm_Select(object sender, EventArgs e)
    {
        var nodeViewModel = (MapNodeVM)sender;
        var eventArgs = new NodeInteractEventArgs(nodeViewModel);
        NodeSelected?.Invoke(this, eventArgs);
    }
}

