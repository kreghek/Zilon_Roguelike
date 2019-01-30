using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.World;

public class GlobeWorldVM : MonoBehaviour
{
    public MapLocation LocationPrefab;
    public MapLocationConnector ConnectorPrefab;
    public GroupVM HumanGroupPrefab;

    [Inject] private readonly IGlobeManager _globeManager;
    [Inject] private readonly HumanPlayer _player;
    [Inject] private readonly DiContainer _container;

    private void Start()
    {
        if (_globeManager.CurrentGlobe == null)
        {
            _globeManager.GenerateGlobe();

            var firstLocality = _globeManager.CurrentGlobe.Localities.First();

            _player.Terrain = firstLocality.Cell;

            var createdRegion = _globeManager.GenerateRegion(firstLocality.Cell);

            _globeManager.Regions[_player.Terrain] = createdRegion;

            var firstNode = (GlobeRegionNode)createdRegion.Nodes.First();

            _player.GlobeNode = firstNode;
        }

        var currentGlobeCell = _player.Terrain;
        var region = _globeManager.Regions[currentGlobeCell];

        var locationNodeViewModels = new List<MapLocation>(100);
        foreach (GlobeRegionNode globeRegionNode in region.Nodes)
        {
            var worldCoords = HexHelper.ConvertToWorld(globeRegionNode.OffsetX, globeRegionNode.OffsetY);

            var locationObject = _container.InstantiatePrefab(LocationPrefab, transform);
            locationObject.transform.position = new Vector3(worldCoords[0], worldCoords[1], 0);
            var locationViewModel = locationObject.GetComponent<MapLocation>();
            locationViewModel.Node = globeRegionNode;
            locationNodeViewModels.Add(locationViewModel);

            locationViewModel.OnSelect += LocationViewModel_OnSelect;
        }

        var openNodeViewModels = new List<MapLocation>(locationNodeViewModels);
        while (openNodeViewModels.Any())
        {
            var currentNodeViewModel = openNodeViewModels[0];
            openNodeViewModels.Remove(currentNodeViewModel);

            var neighbors = region.GetNext(currentNodeViewModel.Node);
            var neighborViewModels = openNodeViewModels.Where(x => neighbors.Contains(x.Node)).ToArray();
            foreach (var neibourNodeViewModel in neighborViewModels)
            {
                var connectorObject = _container.InstantiatePrefab(ConnectorPrefab, transform);
                var connectorViewModel = connectorObject.GetComponent<MapLocationConnector>();
                connectorViewModel.gameObject1 = currentNodeViewModel.gameObject;
                connectorViewModel.gameObject2 = neibourNodeViewModel.gameObject;
            }
        }

        var playerGroupNodeViewModel = locationNodeViewModels.Single(x => x.Node == _player.GlobeNode);
        var groupObject = _container.InstantiatePrefab(HumanGroupPrefab, transform);
        groupObject.transform.position = playerGroupNodeViewModel.transform.position;
    }

    private void LocationViewModel_OnSelect(object sender, System.EventArgs e)
    {
        var selectedNodeViewModel = (MapLocation)sender;

        _player.GlobeNode = selectedNodeViewModel.Node;

        SceneManager.LoadScene("combat");
    }
}