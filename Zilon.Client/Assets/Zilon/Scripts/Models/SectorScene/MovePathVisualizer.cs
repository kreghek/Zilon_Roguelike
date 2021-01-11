using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

public class MovePathVisualizer : MonoBehaviour
{
    public GameObject VisualizationItemPrefab;

    [Inject(Id = "move-command")]
    private readonly ICommand _moveCommand;
    [Inject]
    private readonly ISectorUiState _playerState;

    public void Update()
    {
        ClearPreviousPath();

        if (!(_playerState.HoverViewModel is IMapNodeViewModel))
        {
            return;
        }

        var moveCommand = (MoveCommand)_moveCommand;
        var canMove = _moveCommand.CanExecute();

        if (!canMove)
        {
            // If the player person can move then just returns.
            // There is no path will show because visualization cleared at start of the method.
            return;
        }

        var path = moveCommand.Path;

        if (path == null)
        {
            // If path is null then just clear current path visualization.
            // There is no path will show because visualization cleared at start of the method.
            return;
        }

        // Old path was cleared at start of the method.
        foreach (var pathNode in path)
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
