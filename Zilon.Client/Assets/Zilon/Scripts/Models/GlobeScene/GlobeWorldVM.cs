using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

public class GlobeWorldVM : MonoBehaviour
{
    public MapLocation LocationPrefab;
    public MapLocationConnector ConnectorPrefab;
    public GroupVM HumanGroupPrefab;
    public GlobalFollowCamera Camera;

    private GroupVM _groupViewModel;
    private GlobeRegion _region;
    private List<MapLocation> _locationNodeViewModels;

    [Inject] private readonly IWorldManager _globeManager;
    [Inject] private readonly IWorldGenerator _globeGenerator;
    [Inject] private readonly IScoreManager _scoreManager;
    [Inject] private readonly HumanPlayer _player;
    [Inject] private readonly DiContainer _container;

    private async void Start()
    {
        if (_globeManager.Globe == null)
        {
            _globeManager.Globe = await _globeGenerator.GenerateGlobeAsync();

            var firstLocality = _globeManager.Globe.Localities.First();

            _player.Terrain = firstLocality.Cell;

            var createdRegion = await _globeGenerator.GenerateRegionAsync(_globeManager.Globe, firstLocality.Cell);

            _globeManager.Regions[_player.Terrain] = createdRegion;

            var firstNode = (GlobeRegionNode)createdRegion.Nodes.First();

            _player.GlobeNode = firstNode;
        }

        var currentGlobeCell = _player.Terrain;
        _region = _globeManager.Regions[currentGlobeCell];

        _locationNodeViewModels = new List<MapLocation>(100);
        foreach (GlobeRegionNode globeRegionNode in _region.Nodes)
        {
            var worldCoords = HexHelper.ConvertToWorld(globeRegionNode.OffsetX, globeRegionNode.OffsetY);

            var locationObject = _container.InstantiatePrefab(LocationPrefab, transform);
            locationObject.transform.position = new Vector3(worldCoords[0], worldCoords[1], 0);
            var locationViewModel = locationObject.GetComponent<MapLocation>();
            locationViewModel.Node = globeRegionNode;
            _locationNodeViewModels.Add(locationViewModel);

            locationViewModel.OnSelect += LocationViewModel_OnSelect;
        }

        var openNodeViewModels = new List<MapLocation>(_locationNodeViewModels);
        while (openNodeViewModels.Any())
        {
            var currentNodeViewModel = openNodeViewModels[0];
            openNodeViewModels.Remove(currentNodeViewModel);

            var neighbors = _region.GetNext(currentNodeViewModel.Node);
            var neighborViewModels = openNodeViewModels.Where(x => neighbors.Contains(x.Node)).ToArray();
            foreach (var neibourNodeViewModel in neighborViewModels)
            {
                var connectorObject = _container.InstantiatePrefab(ConnectorPrefab, transform);
                var connectorViewModel = connectorObject.GetComponent<MapLocationConnector>();
                connectorViewModel.gameObject1 = currentNodeViewModel.gameObject;
                connectorViewModel.gameObject2 = neibourNodeViewModel.gameObject;
            }
        }

        var playerGroupNodeViewModel = _locationNodeViewModels.Single(x => x.Node == _player.GlobeNode);
        var groupObject = _container.InstantiatePrefab(HumanGroupPrefab, transform);
        _groupViewModel = groupObject.GetComponent<GroupVM>();
        _groupViewModel.CurrentLocation = playerGroupNodeViewModel;
        groupObject.transform.position = playerGroupNodeViewModel.transform.position;
        Camera.Target = groupObject;
    }

    private void LocationViewModel_OnSelect(object sender, System.EventArgs e)
    {
        var selectedNodeViewModel = (MapLocation)sender;

        var currentNode = _player.GlobeNode;
        var currentNodeViewModel = _locationNodeViewModels.Single(x => x.Node == currentNode);

        var neighborNodes = _region.GetNext(currentNode);
        var selectedIsNeighbor = neighborNodes.Contains(selectedNodeViewModel.Node);

        if (selectedIsNeighbor)
        {
            _player.GlobeNode = selectedNodeViewModel.Node;

            if (_player.GlobeNode.Scheme.SectorLevels != null || _player.GlobeNode.IsTown)
            {
                _scoreManager.CountPlace(selectedNodeViewModel.Node);
                SceneManager.LoadScene("loading");
            }
            else
            {
                _groupViewModel.CurrentLocation = selectedNodeViewModel;
            }
        }
    }
}