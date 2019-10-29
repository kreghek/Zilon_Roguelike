using System.Collections.Generic;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

public class MovePathVisualizer : MonoBehaviour
{
    public GameObject VisualizationItemPrefab;

    [Inject(Id = "move-command")] private readonly ICommand _moveCommand;
    [Inject] private readonly ISectorUiState _playerState;
    private IList<IMapNode> _lastPath;

    public void FixedUpdate()
    {
        var moveCommand = (MoveCommand)_moveCommand;
        var path = moveCommand.Path;

        foreach (Transform visualizationItem in transform)
        {
            Destroy(visualizationItem.gameObject);
        }

        if (_playerState.HoverViewModel is IMapNodeViewModel)
        {
            _lastPath = path;

            if (_lastPath != null)
            {
                foreach (var pathNode in _lastPath)
                {
                    var hexPathNode = (HexNode)pathNode;
                    var worldPosition = HexHelper.ConvertToWorld(hexPathNode.OffsetX, hexPathNode.OffsetY);

                    var item = Instantiate(VisualizationItemPrefab, transform);
                    item.transform.position = new Vector3(worldPosition[0], worldPosition[1] / 2);
                }
            }
        }
    }
}
