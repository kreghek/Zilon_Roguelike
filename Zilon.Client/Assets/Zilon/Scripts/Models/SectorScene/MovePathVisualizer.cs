using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

public class MovePathVisualizer : MonoBehaviour
{
    public GameObject VisualizationItemPrefab;

    [Inject(Id = "move-command")]
    private readonly ICommand _moveCommand;
    [Inject]
    private readonly ISectorUiState _playerState;
    private IList<IGraphNode> _lastPath;

    public void Update()
    {
        if (!(_playerState.HoverViewModel is IMapNodeViewModel))
        {
            ClearPreviousPath();
            return;
        }

        var moveCommand = (MoveCommand)_moveCommand;
        var canMove = _moveCommand.CanExecute();

        if (!canMove)
        {
            ClearPreviousPath();
            return;
        }

        var path = moveCommand.Path;

        if (_lastPath == path)
        {
            // If last and current are equals just do nothing.
            // Because path is not changed so current visualization is correct.
            return;
        }

        _lastPath = path;

        if (path == null || path.Count <= 1)
        {
            // First element of the path is the cell with the player person.
            // If path is null or path length equals 1 then just clear current path visualization.

            ClearPreviousPath();

            return;
        }

        ClearPreviousPath();
        var pathToVisualize = path.Skip(1);

        foreach (var pathNode in pathToVisualize)
        {
            var hexPathNode = (HexNode)pathNode;
            var worldPosition = HexHelper.ConvertToWorld(hexPathNode.OffsetCoords);

            var item = Instantiate(VisualizationItemPrefab, transform);
            item.transform.position = new Vector3(worldPosition[0], worldPosition[1] / 2);
        }
    }

    private void ClearPreviousPath()
    {
        foreach (Transform visualizationItem in transform)
        {
            Destroy(visualizationItem.gameObject);
        }
    }
}
